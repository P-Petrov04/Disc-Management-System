import { useState, useEffect } from "react";
import axios from "axios";
import { getUserRole } from "../utils/auth";

function AllDiscs() {
    const [discs, setDiscs] = useState([]);
    const role = getUserRole();

    const fetchDiscs = async () => {
        try {
            const response = await axios.get("https://localhost:7254/api/discs", {
                headers: {
                    Authorization: `Bearer ${sessionStorage.getItem("token")}`
                }
            });
            setDiscs(response.data.data);
        } catch (error) {
            console.error("Error fetching discs:", error);
        }
    };

    useEffect(() => {
        fetchDiscs(); // Initial fetch

        const interval = setInterval(fetchDiscs, 3000); // Fetch every 3 seconds
        return () => clearInterval(interval); // Cleanup
    }, []);

    return (
        <div>
            <h2>Available Discs</h2>
            <table className="table">
                <thead>
                    <tr>
                        <th>Image</th>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Format</th>
                        <th>Duration</th>
                        {role === "Admin" && <th>Status</th>}
                    </tr>
                </thead>
                <tbody>
                    {discs.map((disc) => (
                        <tr key={disc.id}>
                            <td>{disc.image || "No Image"}</td>
                            <td>{disc.title}</td>
                            <td>{disc.artist}</td>
                            <td>{disc.format}</td>
                            <td>{disc.duration} min</td>
                            {role === "Admin" && <td>{disc.isAvailable ? "Available" : "Rented"}</td>}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default AllDiscs;
