import { useEffect, useState } from "react";
import axios from "axios";
import { getUserRole } from "../utils/auth";
import { useNavigate } from "react-router-dom";

function Home() {
    const [discs, setDiscs] = useState([]);
    const [editDisc, setEditDisc] = useState(null);
    const [editedData, setEditedData] = useState({ title: "", artist: "", format: "", durationMinutes: "", releaseDate: "" });

    const token = localStorage.getItem("token");
    const role = getUserRole();
    const navigate = useNavigate();

    useEffect(() => {
        fetchDiscs();
    }, []);

    const fetchDiscs = () => {
        axios.get("https://localhost:7254/api/discs", {
            headers: { Authorization: `Bearer ${token}` }
        })
            .then(response => {
                setDiscs(response.data.data);
            })
            .catch(error => {
                console.error("Error fetching discs:", error);
            });
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Are you sure you want to delete this disc?")) return;
        try {
            await axios.delete(`https://localhost:7254/api/discs/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            fetchDiscs();
            alert("Disc deleted successfully!");
        } catch (error) {
            console.error("Error deleting disc:", error);
            alert("Failed to delete disc.");
        }
    };

    const handleEdit = (disc) => {
        setEditDisc(disc.discId);
        setEditedData({ ...disc });
    };

    const handleEditChange = (e) => {
        setEditedData({ ...editedData, [e.target.name]: e.target.value });
    };

    const handleEditSubmit = async () => {
        try {
            await axios.put(`https://localhost:7254/api/discs/${editDisc}`, editedData, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setEditDisc(null);
            fetchDiscs();
            alert("Disc updated successfully!");
        } catch (error) {
            console.error("Error updating disc:", error);
            alert("Failed to update disc.");
        }
    };

    return (
        <div>
            <h2>Available Discs</h2>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Image</th>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Format</th>
                        <th>Duration</th>
                        {role === "Admin" && <th>Actions</th>}
                    </tr>
                </thead>
                <tbody>
                    {discs.map(disc => (
                        <tr key={disc.discId}>
                            <td>
                                {disc.photoUrl ? (
                                    <img src={`https://localhost:7254${disc.photoUrl}`} alt={disc.title}
                                        style={{ width: "50px", height: "50px", objectFit: "cover", borderRadius: "5px" }} />
                                ) : "No Image"}
                            </td>
                            <td>{editDisc === disc.discId ? <input type="text" name="title" value={editedData.title} onChange={handleEditChange} /> : disc.title}</td>
                            <td>{editDisc === disc.discId ? <input type="text" name="artist" value={editedData.artist} onChange={handleEditChange} /> : disc.artist}</td>
                            <td>{editDisc === disc.discId ? <input type="text" name="format" value={editedData.format} onChange={handleEditChange} /> : disc.format}</td>
                            <td>{editDisc === disc.discId ? <input type="number" name="durationMinutes" value={editedData.durationMinutes} onChange={handleEditChange} /> : `${disc.durationMinutes} min`}</td>

                            {role === "Admin" && (
                                <td>
                                    {editDisc === disc.discId ? (
                                        <>
                                            <button className="btn btn-success btn-sm me-2" onClick={handleEditSubmit}>Save</button>
                                            <button className="btn btn-secondary btn-sm" onClick={() => setEditDisc(null)}>Cancel</button>
                                        </>
                                    ) : (
                                        <>
                                            <button className="btn btn-warning btn-sm me-2" onClick={() => handleEdit(disc)}>Edit</button>
                                            <button className="btn btn-danger btn-sm" onClick={() => handleDelete(disc.discId)}>Delete</button>
                                        </>
                                    )}
                                </td>
                            )}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default Home;
