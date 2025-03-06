import { jwtDecode } from "jwt-decode";

// Check if the user is authenticated
export const isAuthenticated = () => {
    return localStorage.getItem("token") !== null; // Use localStorage for persistence
};

// Get the user's role from the token
export const getUserRole = () => {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const decodedUser = jwtDecode(token);
        return decodedUser.role || "User";
    } catch (error) {
        console.error("Error decoding token:", error);
        return null;
    }
};

// Set authentication data (token and user info)
export const setAuthData = (token, setAuth) => {
    try {
        const decodedUser = jwtDecode(token);

        const user = {
            userId: decodedUser.nameid,
            firstName: decodedUser.given_name || "Guest",
            email: decodedUser.unique_name,
            role: decodedUser.role
        };

        localStorage.setItem("token", token); // Use localStorage
        localStorage.setItem("user", JSON.stringify(user));

        setAuth(true); // Update auth state in React
    } catch (error) {
        console.error("Error storing auth data:", error);
    }
};

// Get the user's ID
export const getUserId = () => {
    const user = localStorage.getItem("user");
    if (!user) return null;

    try {
        const parsedUser = JSON.parse(user);
        return parsedUser.userId || null;
    } catch (error) {
        console.error("Error parsing user data:", error);
        return null;
    }
};

// Get the user's first name
export const getUserFirstName = () => {
    const user = localStorage.getItem("user");
    if (!user) return "Guest";

    try {
        const parsedUser = JSON.parse(user);
        return parsedUser.firstName || parsedUser.email.split("@")[0] || "Guest";
    } catch (error) {
        console.error("Error parsing user data:", error);
        return "Guest";
    }
};

// Log out the user
export const logout = (setAuth) => {
    localStorage.removeItem("token"); // Clear token
    localStorage.removeItem("user"); // Clear user data
    setAuth(false); // Update auth state in React
};