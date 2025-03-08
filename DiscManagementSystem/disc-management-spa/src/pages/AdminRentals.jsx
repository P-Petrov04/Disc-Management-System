import { useEffect, useState } from "react";
import axios from "axios";

function AdminRentals() {
    const [rentals, setRentals] = useState([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [usersMap, setUsersMap] = useState({});
    const [discsMap, setDiscsMap] = useState({});
    const token = localStorage.getItem("token");

    const fetchUsersAndDiscs = async () => {
        try {
            const [usersResponse, discsResponse] = await Promise.all([
                axios.get(`https://localhost:7254/api/users`, {
                    headers: { Authorization: `Bearer ${token}` }
                }),
                axios.get(`https://localhost:7254/api/discs`, {
                    headers: { Authorization: `Bearer ${token}` }
                })
            ]);

            const users = usersResponse.data.users.reduce((acc, user) => {
                acc[user.userId] = user.firstName || "N/A";
                return acc;
            }, {});

            const discs = discsResponse.data.data.reduce((acc, disc) => {
                acc[disc.discId] = disc.title || "Unknown";
                return acc;
            }, {});

            setUsersMap(users);
            setDiscsMap(discs);
        } catch (error) {
            console.error("Error fetching users and discs:", error);
        }
    };

    const fetchRentals = async (page = 1, size = 5) => {
        try {
            await fetchUsersAndDiscs();

            const response = await axios.get(`https://localhost:7254/api/rentals?pageP=${page}&sizeP=${size}`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            const rentalsWithDetails = response.data.data.map(rental => ({
                ...rental,
                userName: usersMap[rental.userId] || "N/A",
                discName: discsMap[rental.discId] || "Unknown"
            }));

            setRentals(rentalsWithDetails);
            setTotalPages(Math.ceil(response.data.totalRentals / size));
        } catch (error) {
            console.error("Error fetching rentals:", error);
        }
    };

    useEffect(() => {
        fetchRentals(page);

        const interval = setInterval(() => fetchRentals(page), 3000);
        return () => clearInterval(interval);
    }, [page]);

    return (
        <div>
            <h2>All Rentals</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Rental ID</th>
                        <th>User ID</th>
                        <th>User Name</th>
                        <th>Disc ID</th>
                        <th>Disc Name</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {rentals.map(rental => (
                        <tr key={rental.rentalId}>
                            <td>{rental.rentalId}</td>
                            <td>{rental.userId}</td>
                            <td>{rental.userName}</td>
                            <td>{rental.discId}</td>
                            <td>{rental.discName}</td>
                            <td>{rental.status}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <div>
                <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
                    Previous
                </button>
                <span> Page {page} of {totalPages} </span>
                <button onClick={() => setPage(p => p + 1)} disabled={page === totalPages}>
                    Next
                </button>
            </div>
        </div>
    );
}

export default AdminRentals;
