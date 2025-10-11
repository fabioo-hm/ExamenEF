import { API_URL } from "./config.js";
document.getElementById("signUpForm").addEventListener("submit", async function(event) {
    event.preventDefault();
    clearErrorMessages();

    const username = document.getElementById("username").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value.trim();
    const role = document.getElementById("role").value;

    let valid = true;

    if (!username) { showErrorMessage("username", "El nombre de usuario es obligatorio."); valid = false; }
    if (!email) { showErrorMessage("email", "El correo electrónico es obligatorio."); valid = false; }
    else if (!validateEmail(email)) { showErrorMessage("email", "Correo inválido."); valid = false; }
    if (!password) { showErrorMessage("password", "La contraseña es obligatoria."); valid = false; }
    if (!role) { showErrorMessage("role", "Debe seleccionar un rol."); valid = false; }

    if (!valid) return;

    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                username,
                email,
                password,
                role // ← asegúrate que coincida con el nombre del campo que espera tu backend
            })
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || "Error al registrarse");
        }

        alert("Registro exitoso ✅");
        window.location.href = "index.html";

    } catch (error) {
        alert("Error al registrarse: " + error.message);
    }
});

function validateEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

function showErrorMessage(fieldId, message) {
    const field = document.getElementById(fieldId);
    const errorMessage = document.createElement('div');
    errorMessage.classList.add('error-message');
    errorMessage.textContent = message;
    field.parentNode.appendChild(errorMessage);
}

function clearErrorMessages() {
    const errorMessages = document.querySelectorAll('.error-message');
    errorMessages.forEach(msg => msg.remove());
}
