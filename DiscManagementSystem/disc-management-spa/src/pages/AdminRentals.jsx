import { useEffect, useState } from "react";
import axios from "axios";
import { Container, Typography, Card, CardContent, List, ListItem, ListItemText, Chip } from "@mui/material";

function AdminRentals() {
    const [rentals, setRentals] = useState([]);
    const token = localStorage.getItem("token");

    useEffect(() => {
        axios.get("https://localhost:7254/api/rentals", {
            headers: { Authorization: `Bearer ${token}` }
        })
            .then(response => {
                setRentals(response.data.data);
            })
            .catch(error => {
                console.error("Error fetching rentals:", error);
            });
    }, []);

    return (
        <Container maxWidth="md" sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>
                All Rentals
            </Typography>
            <Card variant="outlined">
                <CardContent>
                    <List>
                        {rentals.map(rental => (
                            <ListItem key={rental.rentalId} divider>
                                <ListItemText
                                    primary={`User ${rental.userId} rented Disc ${rental.discId}`}
                                    secondary={`Status: ${rental.status}`}
                                />
                                <Chip
                                    label={rental.status}
                                    color={rental.status === "Active" ? "success" : "default"}
                                    variant="outlined"
                                />
                            </ListItem>
                        ))}
                    </List>
                </CardContent>
            </Card>
        </Container>
    );
}

export default AdminRentals;
