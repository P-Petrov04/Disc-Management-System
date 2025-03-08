import { Link, useNavigate } from "react-router-dom";
import { getUserId, getUserFirstName, getUserRole, logout } from "../utils/auth";

function Navbar({ auth, setAuth }) {
    const firstName = getUserFirstName() || "Guest"; // ✅ Safe fallback
    const userId = getUserId();
    const role = getUserRole();
    const navigate = useNavigate();

    console.log("First Name:", firstName); // Debug: Check the firstName
    console.log("User ID:", userId); // Debug: Check the userId
    console.log("Role:", role); // Debug: Check the role

    const handleLogout = () => {
        logout(setAuth); // ✅ Log out and reset state
        navigate("/login");
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container">
                {auth ? (
                    <>
                        <Link className="navbar-brand" to="/">Disc Management</Link>
                        <div className="collapse navbar-collapse">
                            <ul className="navbar-nav ms-auto">
                                <li className="nav-item">
                                    <Link className="nav-link" to="/all-discs">Available Discs</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/my-rentals">My Rentals</Link>
                                </li>
                                {role === "Admin" && (
                                    <>
                                        <li className="nav-item">
                                            <Link className="nav-link" to="/admin/users">All Users</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link" to="/admin/rentals">All Rentals</Link>
                                        </li>
                                    </>
                                )}
                                <li className="nav-item">
                                    <span className="nav-link">
                                        Welcome, {firstName} {userId ? `(ID: ${userId})` : ""}
                                    </span>
                                </li>
                                <li className="nav-item">
                                    <button className="btn btn-danger" onClick={handleLogout}>
                                        Logout
                                    </button>
                                </li>
                            </ul>
                        </div>
                    </>
                ) : (
                    <h2 className="text-white">Please log in to access the system</h2>
                )}
            </div>
        </nav>
    );
}

export default Navbar;