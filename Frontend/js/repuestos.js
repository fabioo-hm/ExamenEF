import { API_URL } from "./config.js";

// === ELEMENTOS DOM ===
const tablaRepuestos = document.querySelector("#tablaRepuestos tbody");

// Botones principales
const btnAgregar = document.getElementById("btnAgregarRepuesto");
const btnEditar = document.getElementById("btnEditarRepuesto");
const btnEliminar = document.getElementById("btnEliminarRepuesto");

// Modales
const modalCrear = document.getElementById("modalCrearRepuesto");
const modalEditar = document.getElementById("modalEditarRepuesto");
const modalEliminar = document.getElementById("modalEliminarRepuesto");

// Formularios
const formCrear = document.getElementById("formCrearRepuesto");
const formEditar = document.getElementById("formEditarRepuesto");
const formEliminar = document.getElementById("formEliminarRepuesto");

// Botones cerrar
document.getElementById("repuestoCerrarCrear").addEventListener("click", () => modalCrear.style.display = "none");
document.getElementById("repuestoCerrarEditar").addEventListener("click", () => modalEditar.style.display = "none");
document.getElementById("repuestoCerrarEliminar").addEventListener("click", () => modalEliminar.style.display = "none");

// === CARGAR REPUESTOS ===
async function cargarRepuestos() {
  try {
    const res = await fetch(`${API_URL}/spareparts/all`);
    if (!res.ok) throw new Error("Error al obtener los repuestos");
    const data = await res.json();

    tablaRepuestos.innerHTML = "";

    data.forEach(r => {
      const fila = document.createElement("tr");
      fila.innerHTML = `
        <td>${r.id}</td>
        <td>${r.code}</td>
        <td>${r.description}</td>
        <td>$${r.unitPrice?.toFixed(2)}</td>
        <td>${r.stockQuantity}</td>
      `;
      tablaRepuestos.appendChild(fila);
    });
  } catch (error) {
    console.error(error);
    alert("No se pudieron cargar los repuestos.");
  }
}

// === CREAR REPUESTO ===
btnAgregar.addEventListener("click", () => {
  modalCrear.style.display = "flex";
});

formCrear.addEventListener("submit", async e => {
  e.preventDefault();

  const repuesto = {
    code: document.getElementById("repuestoCrearCodigo").value.trim(),
    description: document.getElementById("repuestoCrearDescripcion").value.trim(),
    unitPrice: parseFloat(document.getElementById("repuestoCrearPrecioU").value),
    stockQuantity: parseInt(document.getElementById("repuestoCrearStock").value)
  };

  try {
    const res = await fetch(`${API_URL}/spareparts`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(repuesto)
    });

    if (!res.ok) throw new Error("Error al crear repuesto");

    alert("✅ Repuesto creado correctamente");
    modalCrear.style.display = "none";
    formCrear.reset();
    cargarRepuestos();
  } catch (err) {
    alert("❌ No se pudo crear el repuesto");
  }
});

// === EDITAR REPUESTO ===
btnEditar.addEventListener("click", () => {
  modalEditar.style.display = "flex";
});

formEditar.addEventListener("submit", async e => {
  e.preventDefault();

  const id = document.getElementById("repuestoEditarId").value.trim();
  const repuesto = {
    id,
    code: document.getElementById("repuestoEditarCodigo").value.trim(),
    description: document.getElementById("repuestoEditarDescripcion").value.trim(),
    unitPrice: parseFloat(document.getElementById("repuestoEditarPrecioU").value),
    stockQuantity: parseInt(document.getElementById("repuestoEditarStock").value)
  };

  try {
    const res = await fetch(`${API_URL}/spareparts/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(repuesto)
    });

    if (!res.ok) throw new Error("Error al actualizar repuesto");

    alert("✅ Repuesto actualizado correctamente");
    modalEditar.style.display = "none";
    formEditar.reset();
    cargarRepuestos();
  } catch (err) {
    alert("❌ No se pudo actualizar el repuesto");
  }
});

// === ELIMINAR REPUESTO ===
btnEliminar.addEventListener("click", () => {
  modalEliminar.style.display = "flex";
});

formEliminar.addEventListener("submit", async e => {
  e.preventDefault();

  const id = document.getElementById("repuestoEliminarId").value.trim();

  try {
    const res = await fetch(`${API_URL}/spareparts/${id}`, { method: "DELETE" });

    if (!res.ok) throw new Error("Error al eliminar repuesto");

    alert("🗑️ Repuesto eliminado correctamente");
    modalEliminar.style.display = "none";
    formEliminar.reset();
    cargarRepuestos();
  } catch (err) {
    alert("❌ No se pudo eliminar el repuesto");
  }
});

// === INICIALIZAR ===
cargarRepuestos();
