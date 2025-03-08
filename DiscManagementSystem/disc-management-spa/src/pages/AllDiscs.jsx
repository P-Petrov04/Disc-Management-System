import { useState, useEffect } from "react";
import axios from "axios";
import { getUserRole } from "../utils/auth";

function AllDiscs() {
    const [discs, setDiscs] = useState([]);
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const role = getUserRole();

    const fetchDiscs = async (page = 1, size = 5) => {
        try {
            const response = await axios.get(`https://localhost:7254/api/discs?pageP=${page}&sizeP=${size}`, {
                headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
            });

            // ✅ Add cache-buster to force the browser to refresh the image
            const discsWithCacheBuster = response.data.data.map(disc => ({
                ...disc,
                photoUrl: disc.photoUrl
                    ? `${disc.photoUrl}?t=${new Date().getTime()}` // 🔥 Cache Buster ✅
                    : null
            }));

            setDiscs(discsWithCacheBuster);
            setTotalPages(Math.ceil(response.data.totalDiscs / size));
        } catch (error) {
            console.error("Error fetching discs:", error);
        }
    };

    useEffect(() => {
        fetchDiscs(page);

        // ✅ Keep refreshing every 3 seconds
        const interval = setInterval(() => fetchDiscs(page), 3000);
        return () => clearInterval(interval);
    }, [page]);

    return (
        <div>
            <h2>Available Discs</h2>
            <table className="table">
                <thead>
                    <tr>
                        <th>Image</th>
                        <th>Disc ID</th>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Format</th>
                        <th>Duration</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    {discs.map(disc => (
                        <tr key={disc.discId}>
                            <td>
                                {disc.photoUrl
                                    ? <img src={disc.photoUrl} alt={disc.title} width="50" />
                                    : "No Image"}
                            </td>
                            <td>{disc.discId}</td>
                            <td>{disc.title}</td>
                            <td>{disc.artist}</td>
                            <td>{disc.format}</td>
                            <td>{disc.durationMinutes} min</td>
                            <td>{disc.isAvailable ? "Available" : "Rented"}</td>
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

export default AllDiscs;
