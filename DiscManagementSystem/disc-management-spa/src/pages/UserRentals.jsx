import { useState, useEffect } from "react";
import axios from "axios";
import { getUserId } from "../utils/auth";

function UserRentals() {
    const [rentals, setRentals] = useState([]);
    const userId = getUserId();

    const fetchRentals = async () => {
        if (!userId) return;

        try {
            const response = await axios.get(`https://localhost:7254/api/rentals/my-rentals`, {
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem("token")}`
                }
            });

            if (response.data && Array.isArray(response.data.data)) {
                setRentals(response.data.data);
            } else {
                console.error("Unexpected API response format:", response.data);
                setRentals([]);
            }
        } catch (error) {
            console.error("Error fetching rentals:", error);
            setRentals([]);
        }
    };

    useEffect(() => {
        fetchRentals(); // Initial fetch

        const interval = setInterval(fetchRentals, 3000); // Fetch every 3 seconds
        return () => clearInterval(interval); // Cleanup
    }, [userId]);

    return (
        <div>
            <h2>My Rentals</h2>
            <table className="table">
                <thead>
                    <tr>
                        <th>Disc ID</th>
                        <th>Status</th>
                        <th>Rental Date</th>
                        <th>Return Date</th>
                    </tr>
                </thead>
                <tbody>
                    {rentals.length > 0 ? (
                        rentals.map((rental) => (
                            <tr key={rental.rentalId}>
                                <td>{rental.discId}</td>
                                <td>{rental.status}</td>
                                <td>{rental.rentalDate ? new Date(rental.rentalDate).toLocaleDateString() : "N/A"}</td>
                                <td>{rental.returnDate ? new Date(rental.returnDate).toLocaleDateString() : "Not Returned"}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="4" className="text-center">No rentals found</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}

export default UserRentals;
