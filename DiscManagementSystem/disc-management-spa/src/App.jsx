import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import Home from "./pages/Home";
import CreateDisc from "./pages/CreateDisc";
import NotFound from "./pages/NotFound";
import Navbar from "./components/Navbar";
import Login from "./pages/Login";
import AdminUsers from "./pages/AdminUsers";
import AdminRentals from "./pages/AdminRentals";
import CreateUser from "./pages/CreateUser"; // ✅ Import CreateUser

import { getUserRole } from "./utils/auth";
import "bootstrap/dist/css/bootstrap.min.css";

function App() {
  const role = getUserRole();

  return (
    <Router>
      <Navbar />
      <div className="container mt-4">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />

          {/* Protect routes based on Admin role */}
          <Route path="/create" element={role === "Admin" ? <CreateDisc /> : <Navigate to="/" />} />
          <Route path="/admin/users" element={role === "Admin" ? <AdminUsers /> : <Navigate to="/" />} />
          <Route path="/admin/rentals" element={role === "Admin" ? <AdminRentals /> : <Navigate to="/" />} />
          <Route path="/admin/create-user" element={role === "Admin" ? <CreateUser /> : <Navigate to="/" />} />

          <Route path="*" element={<NotFound />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
