import { useEffect, useState } from "react";
import axios from "axios";

function AdminUsers() {
    const [users, setUsers] = useState([]);
    const token = sessionStorage.getItem("token");

    const fetchUsers = async () => {
        try {
            const response = await axios.get("https://localhost:7254/api/users", {
                headers: { Authorization: `Bearer ${token}` }
            });
            setUsers(response.data.users);
        } catch (error) {
            console.error("Error fetching users:", error);
        }
    };

    useEffect(() => {
        fetchUsers(); // Initial fetch

        const interval = setInterval(fetchUsers, 3000); // Fetch every 3 seconds
        return () => clearInterval(interval); // Cleanup
    }, []);

    return (
        <div>
            <h2>All Users</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Email</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.userId}>
                            <td>{user.email}</td>
                            <td>{user.firstName}</td>
                            <td>{user.lastName}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AdminUsers;
