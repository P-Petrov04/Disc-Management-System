import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { setAuthData } from "../utils/auth";

function Login({ setAuth }) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post("https://localhost:7254/api/auth/login", {
                email,
                password
            }, {
                headers: { "Content-Type": "application/json" }
            });

            const { token } = response.data;

            if (!token) {
                console.error("Token is missing in the response!");
                alert("Login failed. Please try again.");
                return;
            }

            setAuthData(token, setAuth); // Update auth state
            alert("Login successful!");
            navigate("/all-discs"); // Navigate to All Discs
        } catch (error) {
            console.error("Login failed:", error);
            alert("Invalid email or password.");
        }
    };

    return (
        <div>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-3">
                    <label>Email</label>
                    <input type="email" className="form-control" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div className="mb-3">
                    <label>Password</label>
                    <input type="password" className="form-control" value={password} onChange={(e) => setPassword(e.target.value)} required />
                </div>
                <button type="submit" className="btn btn-primary">Login</button>
            </form>
        </div>
    );
}

export default Login;