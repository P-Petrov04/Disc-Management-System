import { Link } from "react-router-dom";
import { getUserRole, isAuthenticated, logout } from "../utils/auth";
import { useNavigate } from "react-router-dom";

function Navbar() {
    const role = getUserRole();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
            <div className="container">
                <Link className="navbar-brand" to="/">Disc Management</Link>
                <div className="collapse navbar-collapse">
                    <ul className="navbar-nav ms-auto">
                        <li className="nav-item">
                            <Link className="nav-link" to="/">Home</Link>
                        </li>

                        {role === "Admin" && (
                            <>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/create">Add Disc</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/admin/users">Manage Users</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/admin/create-user">Add User</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link" to="/admin/rentals">All Rentals</Link>
                                </li>
                            </>
                        )}

                        {isAuthenticated() ? (
                            <li className="nav-item">
                                <button className="btn btn-danger" onClick={handleLogout}>Logout</button>
                            </li>
                        ) : (
                            <li className="nav-item">
                                <Link className="nav-link" to="/login">Login</Link>
                            </li>
                        )}
                    </ul>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
