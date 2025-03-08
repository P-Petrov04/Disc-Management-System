import { jwtDecode } from "jwt-decode";

export const isAuthenticated = () => {
    const token = localStorage.getItem("token");
    if (!token) return false;

    try {
        const decodedUser = jwtDecode(token);
        const currentTime = Date.now() / 1000;

        if (decodedUser.exp && decodedUser.exp < currentTime) {
            logout();
            return false;
        }

        return true;
    } catch (error) {
        console.error("Error decoding token:", error);
        logout();
        return false;
    }
};

export const getUserRole = () => {
    const token = localStorage.getItem("token");
    if (!token) return null;

    try {
        const decodedUser = jwtDecode(token);
        return decodedUser.role || "User";
    } catch {
        return null;
    }
};

export const setAuthData = (token, setAuth) => {
    try {
        const decodedUser = jwtDecode(token);
        const user = {
            userId: decodedUser.nameid,
            firstName: decodedUser.given_name || decodedUser.unique_name?.split("@")[0] || "Guest",
            email: decodedUser.unique_name,
            role: decodedUser.role
        };

        localStorage.setItem("token", token);
        localStorage.setItem("user", JSON.stringify(user));
        setAuth(true);
    } catch {
        logout();
    }
};

export const getUserId = () => {
    const user = localStorage.getItem("user");
    if (!user) return null;

    try {
        const parsedUser = JSON.parse(user);
        return parsedUser.userId || null;
    } catch {
        return null;
    }
};

export const getUserFirstName = () => {
    const user = localStorage.getItem("user");
    if (!user) return "Guest";

    try {
        const parsedUser = JSON.parse(user);
        return parsedUser.firstName || parsedUser.email?.split("@")[0] || "Guest";
    } catch {
        return "Guest";
    }
};

export const logout = (setAuth) => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    if (setAuth) setAuth(false);
};
