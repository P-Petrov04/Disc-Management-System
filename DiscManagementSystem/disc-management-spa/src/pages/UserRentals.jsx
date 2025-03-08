import { useState, useEffect } from "react";
import axios from "axios";
import { getUserId } from "../utils/auth";

function UserRentals() {
    const [rentals, setRentals] = useState([]);
    const userId = getUserId();
    const token = localStorage.getItem("token");

    const fetchRentals = async () => {
        if (!userId || !token) return;

        try {
            const rentalsResponse = await axios.get(`https://localhost:7254/api/rentals/my-rentals`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            const discsResponse = await axios.get(`https://localhost:7254/api/discs`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            // ✅ Map disc ID to disc names
            const discsMap = discsResponse.data.data.reduce((acc, disc) => {
                acc[disc.discId] = disc.title || "Unknown";
                return acc;
            }, {});

            // ✅ Add disc names to rentals
            const rentalsWithDiscNames = rentalsResponse.data.data.map(rental => ({
                ...rental,
                discName: discsMap[rental.discId] || "Unknown"
            }));

            setRentals(rentalsWithDiscNames);
        } catch (error) {
            console.error("Error fetching rentals:", error);
        }
    };

    useEffect(() => {
        fetchRentals();
        const interval = setInterval(fetchRentals, 3000);
        return () => clearInterval(interval);
    }, [userId]);

    return (
        <div>
            <h2>My Rentals</h2>
            <table className="table">
                <thead>
                    <tr>
                        <th>Disc ID</th>
                        <th>Disc Name</th>
                        <th>Status</th>
                        <th>Rental Date</th>
                        <th>Return Date</th>
                    </tr>
                </thead>
                <tbody>
                    {rentals.map(rental => (
                        <tr key={rental.rentalId}>
                            <td>{rental.discId}</td>
                            <td>{rental.discName}</td>
                            <td>{rental.status}</td>
                            <td>{new Date(rental.rentalDate).toLocaleDateString()}</td>
                            <td>{rental.returnDate ? new Date(rental.returnDate).toLocaleDateString() : "Not Returned"}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default UserRentals;
