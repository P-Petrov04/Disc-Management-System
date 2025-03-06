import { Link, useNavigate } from "react-router-dom";
import { getUserId, getUserFirstName, getUserRole, logout } from "../utils/auth";

function Navbar({ auth, setAuth }) {
    const firstName = getUserFirstName();
    const userId = getUserId();
    const role = getUserRole();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout(setAuth); // Log out and update auth state
        navigate("/login"); // Navigate to login without refreshing
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container">
                {auth ? (
                    <>
                        <Link className="navbar-brand" to="/">Disc Management</Link>
                        <div className="collapse navbar-collapse">
                            <ul className="navbar-nav ms-auto">
                                <li className="nav-item"><Link className="nav-link" to="/all-discs">Available Discs</Link></li>
                                <li className="nav-item"><Link className="nav-link" to="/my-rentals">My Rentals</Link></li>
                                {role === "Admin" && (
                                    <>
                                        <li className="nav-item"><Link className="nav-link" to="/admin/users">All Users</Link></li>
                                        <li className="nav-item"><Link className="nav-link" to="/admin/rentals">All Rentals</Link></li>
                                    </>
                                )}
                                <li className="nav-item">
                                    <span className="nav-link">Welcome, {firstName} (ID: {userId})</span>
                                </li>
                                <li className="nav-item"><button className="btn btn-danger" onClick={handleLogout}>Logout</button></li>
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