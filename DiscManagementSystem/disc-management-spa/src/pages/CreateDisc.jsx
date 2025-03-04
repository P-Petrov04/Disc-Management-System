import { useState } from "react";
import axios from "axios";

function CreateDisc() {
    const [formData, setFormData] = useState({
        title: "",
        artist: "",
        format: "",
        durationMinutes: "",
        releaseDate: "",
        photo: null
    });

    const handleChange = (e) => {
        if (e.target.name === "photo") {
            setFormData({ ...formData, photo: e.target.files[0] });
        } else {
            setFormData({ ...formData, [e.target.name]: e.target.value });
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const token = localStorage.getItem("token");

            // Convert to FormData for file upload
            const formDataToSend = new FormData();
            formDataToSend.append("title", formData.title);
            formDataToSend.append("artist", formData.artist);
            formDataToSend.append("format", formData.format);
            formDataToSend.append("durationMinutes", formData.durationMinutes);
            formDataToSend.append("releaseDate", formData.releaseDate);
            if (formData.photo) {
                formDataToSend.append("photo", formData.photo);
            }

            const response = await axios.post("https://localhost:7254/api/discs/create", formDataToSend, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "multipart/form-data"
                }
            });

            alert("Disc created successfully!");
            setFormData({ // Reset form fields
                title: "",
                artist: "",
                format: "",
                durationMinutes: "",
                releaseDate: "",
                photo: null
            });

        } catch (error) {
            console.error("Error adding disc:", error);
            alert("Failed to create disc.");
        }
    };

    return (
        <div>
            <h2>Add New Disc</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label className="form-label">Title</label>
                    <input type="text" name="title" className="form-control" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                    <label className="form-label">Artist</label>
                    <input type="text" name="artist" className="form-control" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                    <label className="form-label">Format</label>
                    <input type="text" name="format" className="form-control" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                    <label className="form-label">Duration (minutes)</label>
                    <input type="number" name="durationMinutes" className="form-control" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                    <label className="form-label">Release Date</label>
                    <input type="date" name="releaseDate" className="form-control" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                    <label className="form-label">Upload Image</label>
                    <input type="file" name="photo" className="form-control" onChange={handleChange} accept="image/*" />
                </div>
                <button type="submit" className="btn btn-primary">Add Disc</button>
            </form>
        </div>
    );
}

export default CreateDisc;
