import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { useEffect, useState } from "react";
import AllDiscs from "./pages/AllDiscs";
import Login from "./pages/Login";
import AdminUsers from "./pages/AdminUsers";
import AdminRentals from "./pages/AdminRentals";
import UserRentals from "./pages/UserRentals";
import NotFound from "./pages/NotFound";
import Navbar from "./components/Navbar";
import { isAuthenticated } from "./utils/auth";

function App() {
  const [auth, setAuth] = useState(isAuthenticated());

  // Listen for authentication changes
  useEffect(() => {
    const handleStorageChange = () => {
      setAuth(isAuthenticated());
    };

    window.addEventListener("storage", handleStorageChange);
    return () => window.removeEventListener("storage", handleStorageChange);
  }, []);

  return (
    <Router>
      <Navbar auth={auth} setAuth={setAuth} />
      <div className="container mt-4">
        <Routes>
          <Route path="/login" element={<Login setAuth={setAuth} />} />
          <Route path="/" element={auth ? <AllDiscs /> : <Navigate replace to="/login" />} />
          <Route path="/all-discs" element={auth ? <AllDiscs /> : <Navigate replace to="/login" />} />
          <Route path="/my-rentals" element={auth ? <UserRentals /> : <Navigate replace to="/login" />} />
          <Route path="/admin/users" element={auth ? <AdminUsers /> : <Navigate replace to="/login" />} />
          <Route path="/admin/rentals" element={auth ? <AdminRentals /> : <Navigate replace to="/login" />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;