import { API_URL } from "./config.js";

const tablaVehiculos = document.querySelector("#tablaVehiculos tbody");
const btnAgregarVehiculo = document.getElementById("btnAgregarVehiculo");
const btnEditarVehiculo = document.getElementById("btnEditarVehiculo");
const btnEliminarVehiculo = document.getElementById("btnEliminarVehiculo");

// MODALES
const modalCrearVehiculo = document.getElementById("modalCrearVehiculo");
const modalEditarVehiculo = document.getElementById("modalEditarVehiculo");
const modalEliminarVehiculo = document.getElementById("modalEliminarVehiculo");

// FORMULARIOS
const formCrearVehiculo = document.getElementById("formCrearVehiculo");
const formEditarVehiculo = document.getElementById("formEditarVehiculo");
const formEliminarVehiculo = document.getElementById("formEliminarVehiculo");

async function cargarVehiculos() {
  try {
    const res = await fetch(`${API_URL}/vehicles/all`);
    if (!res.ok) throw new Error("Error al obtener vehículos");
    const data = await res.json();

    tablaVehiculos.innerHTML = data
      .map(
        (v) => `
      <tr>
        <td>${v.id}</td>
        <td>${v.vin}</td>
        <td>${v.brand}</td>
        <td>${v.model}</td>
        <td>${v.year}</td>
        <td>${v.mileage}</td>
        <td>${v.customerId || "No asignado"}</td>
      </tr>`
      )
      .join("");
  } catch (error) {
    console.error("Error cargando vehículos:", error);
  }
}
formEditarVehiculo.addEventListener("submit", async (e) => {
  e.preventDefault();

  const id = document.getElementById("vehiculoEditarId").value;
  const vehiculoEditado = {
    vin: document.getElementById("vehiculoEditarPlaca").value,
    brand: document.getElementById("vehiculoEditarMarca").value,
    model: document.getElementById("vehiculoEditarModelo").value,
    year: parseInt(document.getElementById("vehiculoEditarAno").value),
    mileage: parseFloat(document.getElementById("vehiculoEditarKilometraje").value)
  };

  try {
    const res = await fetch(`${API_URL}/vehicles/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(vehiculoEditado),
    });

    if (!res.ok) throw new Error("Error al editar vehículo");
    alert("Vehículo actualizado correctamente");
    modalEditarVehiculo.style.display = "none";
    formEditarVehiculo.reset();
    cargarVehiculos();
  } catch (error) {
    console.error("Error:", error);
    alert("No se pudo editar el vehículo");
  }
});

// === CREAR VEHÍCULO ===
formCrearVehiculo.addEventListener("submit", async (e) => {
  e.preventDefault();

  const nuevoVehiculo = {
    vin: document.getElementById("vehiculoCrearPlaca").value,
    brand: document.getElementById("vehiculoCrearMarca").value,
    model: document.getElementById("vehiculoCrearModelo").value,
    year: parseInt(document.getElementById("vehiculoCrearAno").value),
    mileage: parseFloat(document.getElementById("vehiculoCrearKilometraje").value),
    customerId: document.getElementById("vehiculoCrearClienteId").value
  };

  try {
    const res = await fetch(`${API_URL}/vehicles`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(nuevoVehiculo),
    });

    if (!res.ok) throw new Error("Error al crear vehículo");
    alert("Vehículo creado correctamente");
    modalCrearVehiculo.style.display = "none";
    formCrearVehiculo.reset();
    cargarVehiculos();
  } catch (error) {
    console.error("Error:", error);
    alert("No se pudo registrar el vehículo");
  }
});

// === ELIMINAR VEHÍCULO ===
formEliminarVehiculo.addEventListener("submit", async (e) => {
  e.preventDefault();
  const id = document.getElementById("vehiculoEliminarId").value;

  try {
    const res = await fetch(`${API_URL}/vehicles/${id}`, { method: "DELETE" });
    if (!res.ok) throw new Error("Error al eliminar vehículo");
    alert("Vehículo eliminado correctamente");
    modalEliminarVehiculo.style.display = "none";
    formEliminarVehiculo.reset();
    cargarVehiculos();
  } catch (error) {
    console.error("Error:", error);
    alert("No se pudo eliminar el vehículo");
  }
});

// === CONTROL DE MODALES ===
btnAgregarVehiculo.onclick = () => (modalCrearVehiculo.style.display = "flex");
btnEditarVehiculo.onclick = () => (modalEditarVehiculo.style.display = "flex");
btnEliminarVehiculo.onclick = () => (modalEliminarVehiculo.style.display = "flex");

document.getElementById("vehiculoCerrarCrear").addEventListener("click", () => modalCrearVehiculo.style.display = "none");
document.getElementById("vehiculoCerrarEditar").addEventListener("click", () => modalEditarVehiculo.style.display = "none");
document.getElementById("vehiculoCerrarEliminar").addEventListener("click", () => modalEliminarVehiculo.style.display = "none");

// === CARGA INICIAL ===
cargarVehiculos();