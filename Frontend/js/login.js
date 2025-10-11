import { API_URL } from "./config.js";

document.getElementById("loginForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const username = document.getElementById("email").value.trim(); // <-- usa el campo email como username visual
    const password = document.getElementById("password").value.trim();

    if (!username || !password) {
        alert("Por favor, complete todos los campos.");
        return;
    }

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }) // 👈 aquí el cambio importante
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Error en el inicio de sesión");
        }

        const data = await response.json();

        // Guarda el token (ajusta según tu respuesta real del backend)
        const token = data.token || data.accessToken || null;

        if (token) {
            localStorage.setItem("token", token);
            alert("Login exitoso 🎉 Bienvenido " + (data.userName || username));
            window.location.href = "main.html";
        } else {
            alert("No se recibió token. Verifica el backend.");
        }

    } catch (error) {
        alert("Error al iniciar sesión: " + error.message);
    }
});
