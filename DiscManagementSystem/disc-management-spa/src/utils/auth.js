import { jwtDecode } from "jwt-decode";



export const getUserRole = () => {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const decoded = jwtDecode(token);
        return decoded.role;
    } catch (error) {
        return null;
    }
};

export const isAuthenticated = () => {
    return localStorage.getItem("token") !== null;
};

export const logout = () => {
    localStorage.removeItem("token");
};
