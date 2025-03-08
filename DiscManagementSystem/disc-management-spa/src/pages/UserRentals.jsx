import { useState, useEffect } from "react";
import axios from "axios";
import { getUserId } from "../utils/auth";

function AdminMyRentals() {
    const [rentals, setRentals] = useState([]);
    const [discsMap, setDiscsMap] = useState({});
    const [page, setPage] = useState(1); // ✅ Следим текущата страница
    const [totalPages, setTotalPages] = useState(1); // ✅ Следим общия брой страници
    const userId = getUserId();
    const token = localStorage.getItem("token");

    // ✅ Взимаме дисковете и ги пазим в map
    const fetchDiscs = async () => {
        try {
            const discsResponse = await axios.get(`https://localhost:7254/api/discs`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            const discs = discsResponse.data.data.reduce((acc, disc) => {
                acc[disc.discId] = disc.title || "Unknown";
                return acc;
            }, {});

            setDiscsMap(discs);
        } catch (error) {
            console.error("Error fetching discs:", error);
        }
    };

    // ✅ Взимаме само ренталите на логнатия админ с пагинация
    const fetchRentals = async (page = 1, size = 5) => {
        if (!userId || !token) return;

        try {
            await fetchDiscs();

            const rentalsResponse = await axios.get(`https://localhost:7254/api/rentals/my-rentals?page=${page}&size=${size}`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            const rentalsWithDetails = rentalsResponse.data.data.map(rental => ({
                ...rental,
                discName: discsMap[rental.discId] || "Unknown"
            }));

            setRentals(rentalsWithDetails);
            setTotalPages(Math.ceil(rentalsResponse.data.totalRentals / size)); // ✅ Обновяваме totalPages
        } catch (error) {
            console.error("Error fetching admin rentals:", error);
        }
    };

    useEffect(() => {
        fetchRentals(page);

        // ✅ Keep refreshing every 3 sec
        const interval = setInterval(() => fetchRentals(page), 3000);
        return () => clearInterval(interval);
    }, [page, userId, token]);

    return (
        <div>
            <h2>My Rentals</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Rental ID</th>
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
                            <td>{rental.rentalId}</td>
                            <td>{rental.discId}</td>
                            <td>{rental.discName}</td>
                            <td>{rental.status}</td>
                            <td>{new Date(rental.rentalDate).toLocaleDateString()}</td>
                            <td>{rental.returnDate ? new Date(rental.returnDate).toLocaleDateString() : "Not Returned"}</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* ✅ Пагинация */}
            <div>
                <button
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                    disabled={page === 1}
                >
                    Previous
                </button>
                <span> Page {page} of {totalPages} </span>
                <button
                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                    disabled={page === totalPages}
                >
                    Next
                </button>
            </div>
        </div>
    );
}

export default AdminMyRentals;
