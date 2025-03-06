import { useEffect, useState } from "react";
import axios from "axios";

function AdminRentals() {
    const [rentals, setRentals] = useState([]);
    const token = sessionStorage.getItem("token");

    const fetchRentals = async () => {
        try {
            const response = await axios.get("https://localhost:7254/api/rentals", {
                headers: { Authorization: `Bearer ${token}` }
            });
            setRentals(response.data.data);
        } catch (error) {
            console.error("Error fetching rentals:", error);
        }
    };

    useEffect(() => {
        fetchRentals(); // Initial fetch

        const interval = setInterval(fetchRentals, 3000); // Fetch every 3 seconds
        return () => clearInterval(interval); // Cleanup
    }, []);

    return (
        <div>
            <h2>All Rentals</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>User ID</th>
                        <th>Disc ID</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {rentals.map(rental => (
                        <tr key={rental.rentalId}>
                            <td>{rental.userId}</td>
                            <td>{rental.discId}</td>
                            <td>{rental.status}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AdminRentals;
