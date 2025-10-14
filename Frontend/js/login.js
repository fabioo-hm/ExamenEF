import { API_URL } from "./config.js";

const ROLE_REDIRECT = {
  admin: "admin.html",
  recepcionista: "receptionist.html",
  mecanico: "mechanic.html"
};

document.getElementById("loginForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const username = document.getElementById("email").value.trim(); 
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        alert("Por favor, complete todos los campos.");
        return;
    }

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }) 
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Error en el inicio de sesión");
        }

        const data = await response.json();

        
        localStorage.setItem("token", data.token);
        localStorage.setItem("refreshToken", data.refreshToken);
        localStorage.setItem("username", data.userName);

        
        const roles = data.roles || [];
        if (roles.length === 0) {
        alert("No se detectó ningún rol para este usuario.");
        return;
        }

        const role = roles[0].toLowerCase();
        localStorage.setItem("role", role);

        
        const redirectPage = ROLE_REDIRECT[role] || "login.html";
        alert(`Bienvenido ${data.userName} (${role})`);
        window.location.href = redirectPage;

    } catch (error) {
        alert("Error al iniciar sesión: " + error.message);
    }
});
