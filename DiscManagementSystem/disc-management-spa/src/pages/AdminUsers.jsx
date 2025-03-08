import { useState, useEffect } from "react";
import axios from "axios";

function AdminUsers() {
    const [users, setUsers] = useState([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const token = localStorage.getItem("token");

    const fetchUsers = async (page = 1, size = 5) => {
        try {
            const response = await axios.get(`https://localhost:7254/api/users?pageP=${page}&sizeP=${size}`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.data && Array.isArray(response.data.users)) {
                setUsers(response.data.users);
                setTotalPages(Math.ceil(response.data.totalUsers / size));
            }
        } catch (error) {
            console.error("Error fetching users:", error);
            setUsers([]);
        }
    };

    useEffect(() => {
        fetchUsers(page);
        const interval = setInterval(() => fetchUsers(page), 3000);
        return () => clearInterval(interval);
    }, [page]);

    return (
        <div>
            <h2>All Users</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>User ID</th>
                        <th>Email</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map(user => (
                        <tr key={user.userId}>
                            <td>{user.userId}</td>
                            <td>{user.email}</td>
                            <td>{user.firstName || "N/A"}</td>
                            <td>{user.lastName || "N/A"}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div>
                <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
                    Previous
                </button>
                <span> Page {page} of {totalPages} </span>
                <button onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
                    Next
                </button>
            </div>
        </div>
    );
}

export default AdminUsers;
