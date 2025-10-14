import { API_URL } from "./config.js";

const tablaEmpleados = document.querySelector("#tablaEmpleados tbody");
const btnAgregarEmpleado = document.getElementById("btnAgregarEmpleado");
const btnEditarEmpleado = document.getElementById("btnEditarEmpleado");
const btnEliminarEmpleado = document.getElementById("btnEliminarEmpleado");


const modalCrear = document.getElementById("modalCrearEmpleado");
const modalEditar = document.getElementById("modalEditarEmpleado");
const modalEliminar = document.getElementById("modalEliminarEmpleado");

const formCrear = document.getElementById("formCrearEmpleado");
const formEditar = document.getElementById("formEditarEmpleado");
const formEliminar = document.getElementById("formEliminarEmpleado");


function abrirModal(modal) {
  modal.style.display = "block";
}
function cerrarModal(modal) {
  modal.style.display = "none";
}


btnAgregarEmpleado.addEventListener("click", () => abrirModal(modalCrear));
btnEditarEmpleado.addEventListener("click", () => abrirModal(modalEditar));
btnEliminarEmpleado.addEventListener("click", () => abrirModal(modalEliminar));

document.getElementById("empleadoCerrarCrear").addEventListener("click", () => cerrarModal(modalCrear));
document.getElementById("empleadoCerrarEditar").addEventListener("click", () => cerrarModal(modalEditar));
document.getElementById("empleadoCerrarEliminar").addEventListener("click", () => cerrarModal(modalEliminar));


async function cargarEmpleados() {
  try {
    const res = await fetch(`${API_URL}/auth/users`);
    if (!res.ok) throw new Error("Error al obtener empleados");

    const empleados = await res.json();

    tablaEmpleados.innerHTML = "";
    empleados.forEach(emp => {
      const roles = emp.roles?.join(", ") || "Sin rol";
      tablaEmpleados.innerHTML += `
        <tr>
          <td>${emp.id}</td>
          <td>${emp.username}</td>
          <td>${emp.email}</td>
          <td>${roles}</td>
        </tr>
      `;
    });
  } catch (err) {
    alert("❌ " + err.message);
  }
}


formCrear.addEventListener("submit", async e => {
  e.preventDefault();

  const username = document.getElementById("empleadoCrearUsuario").value.trim();
  const email = document.getElementById("empleadoCrearEmail").value.trim();
  const password = document.getElementById("empleadoCrearPassword").value.trim();
  const role = document.getElementById("empleadoCrearRol").value.trim();

  
  if (!username) {
    alert("❌ El campo Usuario es requerido");
    return;
  }

  if (!email) {
    alert("❌ El campo Email es requerido");
    return;
  }

  if (!password) {
    alert("❌ El campo Contraseña es requerido");
    return;
  }

  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(email)) {
    alert("❌ Por favor ingresa un email válido");
    return;
  }

  const dto = {
    username: username,
    email: email,
    password: password,
    role: role || "User" 
  };

  console.log("📤 Enviando datos:", dto);

  try {
    const res = await fetch(`${API_URL}/auth/register`, {
      method: "POST",
      headers: { 
        "Content-Type": "application/json",
        "Accept": "application/json"
      },
      body: JSON.stringify(dto)
    });

    const responseText = await res.text();
    console.log("📥 Status:", res.status, "Respuesta:", responseText);

    if (!res.ok) {
      let errorMessage = "Error del servidor";
      try {
        const errorData = JSON.parse(responseText);
        errorMessage = errorData.message || errorData.Message || errorMessage;
      } catch {
        errorMessage = responseText || `Error ${res.status}: ${res.statusText}`;
      }
      throw new Error(errorMessage);
    }

    const data = JSON.parse(responseText);
    alert((data.message || "Empleado creado correctamente"));
    cerrarModal(modalCrear);
    formCrear.reset();
    await cargarEmpleados();
    
  } catch (err) {
    console.error("💥 Error completo:", err);
    alert("❌ Error al crear empleado: " + err.message);
  }
});

formEditar.addEventListener("submit", async e => {
  e.preventDefault();

  const id = parseInt(document.getElementById("empleadoEditarId").value);
  if (isNaN(id)) return alert("Debe ingresar un ID válido.");

  const dto = {
    username: document.getElementById("empleadoEditarUsuario").value.trim(),
    email: document.getElementById("empleadoEditarEmail").value.trim(),
    password: document.getElementById("empleadoEditarPassword").value.trim(),
    role: document.getElementById("empleadoEditarRol").value.trim()
  };

  console.log("📤 Enviando datos de edición:", dto);

  try {
    const res = await fetch(`${API_URL}/auth/users/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(dto)
    });

    const responseText = await res.text();
    console.log("📥 Respuesta edición:", responseText);

    if (!res.ok) {
      let errorMessage = "Error del servidor";
      try {
        const errorData = JSON.parse(responseText);
        errorMessage = errorData.message || errorData.Message || errorMessage;
      } catch {
        errorMessage = responseText || `Error ${res.status}: ${res.statusText}`;
      }
      throw new Error(errorMessage);
    }

    const data = JSON.parse(responseText);
    alert("✅ " + (data.message || "Empleado actualizado"));

    cerrarModal(modalEditar);
    formEditar.reset();
    await cargarEmpleados();
  } catch (err) {
    console.error("💥 Error en edición:", err);
    alert("❌ Error al actualizar: " + err.message);
  }
});


formEliminar.addEventListener("submit", async e => {
  e.preventDefault();

  const id = parseInt(document.getElementById("empleadoEliminarId").value);
  if (isNaN(id)) return alert("Debe ingresar un ID válido.");

  if (!confirm("¿Seguro que deseas eliminar este empleado?")) return;

  try {
    const res = await fetch(`${API_URL}/auth/users/${id}`, {
      method: "DELETE"
    });

    if (!res.ok) {
      const errorText = await res.text();
      throw new Error(errorText || "Error del servidor");
    }

    const data = await res.json();
    alert("✅ " + (data.message || "Empleado eliminado correctamente"));

    cerrarModal(modalEliminar);
    formEliminar.reset();
    cargarEmpleados();
  } catch (err) {
    alert("❌ Error al eliminar empleado: " + err.message);
  }
});


document.addEventListener("DOMContentLoaded", cargarEmpleados);