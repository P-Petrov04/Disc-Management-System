import { useEffect, useState } from "react";
import axios from "axios";

function AdminUsers() {
    const [users, setUsers] = useState([]);
    const [editUser, setEditUser] = useState(null);
    const [editedData, setEditedData] = useState({ firstName: "", lastName: "", email: "", password: "", phoneNumber: "" });

    const token = localStorage.getItem("token");

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = () => {
        axios.get("https://localhost:7254/api/users", {
            headers: { Authorization: `Bearer ${token}` }
        })
            .then(response => {
                setUsers(response.data.users);
            })
            .catch(error => {
                console.error("Error fetching users:", error);
            });
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this user?")) return;
        try {
            await axios.delete(`https://localhost:7254/api/users/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            fetchUsers();
            alert("User deleted successfully!");
        } catch (error) {
            console.error("Error deleting user:", error);
            alert("Failed to delete user.");
        }
    };

    const handleEdit = (user) => {
        setEditUser(user.userId);
        setEditedData({ ...user, password: "" });
    };

    const handleEditChange = (e) => {
        setEditedData({ ...editedData, [e.target.name]: e.target.value });
    };

    const handleEditSubmit = async () => {
        try {
            await axios.put(`https://localhost:7254/api/users/${editUser}`, editedData, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setEditUser(null);
            fetchUsers();
            alert("User updated successfully!");
        } catch (error) {
            console.error("Error updating user:", error);
            alert("Failed to update user.");
        }
    };

    return (
        <div>
            <h2>Manage Users</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Email</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.userId}>
                            <td>{user.email}</td>
                            <td>{editUser === user.userId ? <input type="text" name="firstName" value={editedData.firstName} onChange={handleEditChange} /> : user.firstName}</td>
                            <td>{editUser === user.userId ? <input type="text" name="lastName" value={editedData.lastName} onChange={handleEditChange} /> : user.lastName}</td>
                            <td>
                                {editUser === user.userId ? (
                                    <>
                                        <button className="btn btn-success btn-sm me-2" onClick={handleEditSubmit}>Save</button>
                                        <button className="btn btn-secondary btn-sm" onClick={() => setEditUser(null)}>Cancel</button>
                                    </>
                                ) : (
                                    <>
                                        <button className="btn btn-warning btn-sm me-2" onClick={() => handleEdit(user)}>Edit</button>
                                        <button className="btn btn-danger btn-sm" onClick={() => handleDelete(user.userId)}>Delete</button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AdminUsers;
