import { API_URL } from "./config.js"; 
// En config.js debería estar: export const API_URL = "http://localhost:5104/api";

document.addEventListener("DOMContentLoaded", async () => {
  const tablaBody = document.querySelector("#tablaClientes tbody");

  try {
    // 1️⃣ Traer clientes y vehículos
    const [clientesRes, vehiculosRes] = await Promise.all([
      fetch(`${API_URL}/customers/all`),
      fetch(`${API_URL}/vehicles/all`)
    ]);

    if (!clientesRes.ok || !vehiculosRes.ok) {
      throw new Error("Error al obtener datos del servidor");
    }

    const clientes = await clientesRes.json();
    const vehiculos = await vehiculosRes.json();

    // 2️⃣ Relacionar clientes con sus vehículos
    const filasHTML = clientes.map(cliente => {
      // Buscar los vehículos que tienen el mismo ID de cliente
      const vehiculosCliente = vehiculos.filter(v => v.customerId === cliente.id);
      const vinList = vehiculosCliente.map(v => v.vin || v.placa || "—").join(", ");

      return `
        <tr>
          <td>${cliente.id}</td>
          <td>${cliente.name || "Sin nombre"}</td>
          <td>${cliente.phone || "—"}</td>
          <td>${cliente.email || "—"}</td>
          <td>${vinList || "Sin vehículo"}</td>
        </tr>
      `;
    }).join("");

    // 3️⃣ Insertar filas en la tabla
    tablaBody.innerHTML = filasHTML;

  } catch (error) {
    console.error("Error cargando clientes:", error);
    tablaBody.innerHTML = `<tr><td colspan="5" style="text-align:center;color:red;">Error al cargar los datos</td></tr>`;
  }
});
// === MODALES ===
const modalCrear = document.getElementById("modalCrearCliente");
const modalEditar = document.getElementById("modalEditarCliente");
const modalEliminar = document.getElementById("modalEliminarCliente");

// Botones abrir modales
document.getElementById("btnAgregarCliente").addEventListener("click", () => modalCrear.style.display = "flex");
document.getElementById("btnEditarCliente").addEventListener("click", () => modalEditar.style.display = "flex");
document.getElementById("btnEliminarCliente").addEventListener("click", () => modalEliminar.style.display = "flex");

// Botones cerrar modales
document.getElementById("clienteCerrarCrear").addEventListener("click", () => modalCrear.style.display = "none");
document.getElementById("clienteCerrarEditar").addEventListener("click", () => modalEditar.style.display = "none");
document.getElementById("clienteCerrarEliminar").addEventListener("click", () => modalEliminar.style.display = "none");

// === CREAR CLIENTE + VEHÍCULO ===
document.getElementById("formCrearCliente").addEventListener("submit", async (e) => {
  e.preventDefault();

  // Datos del cliente
  const nuevoCliente = {
    name: document.getElementById("clienteCrearNombre").value,
    phone: document.getElementById("clienteCrearTelefono").value,
    email: document.getElementById("clienteCrearEmail").value
  };

  // Datos del vehículo
  const nuevoVehiculo = {
    vin: document.getElementById("clienteCrearPlaca").value,
    brand: document.getElementById("clienteCrearMarca").value,
    model: document.getElementById("clienteCrearModelo").value,
    year: parseInt(document.getElementById("clienteCrearAno").value), // ← int
    mileage: parseFloat(document.getElementById("clienteCrearKilometraje").value) // ← double
  };

  try {
    // 1️⃣ Crear cliente
    const resCliente = await fetch(`${API_URL}/customers`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(nuevoCliente)
    });

    if (!resCliente.ok) throw new Error("Error al crear cliente");

    const clienteCreado = await resCliente.json();
    const clienteId = clienteCreado.id || clienteCreado.customerId; // según backend

    // 2️⃣ Crear vehículo vinculado
    const vehiculoConCliente = { ...nuevoVehiculo, customerId: clienteId };

    const resVehiculo = await fetch(`${API_URL}/vehicles`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(vehiculoConCliente)
    });

    if (!resVehiculo.ok) throw new Error("Error al registrar vehículo");

    alert("✅ Cliente y vehículo registrados correctamente");
    modalCrear.style.display = "none";
    location.reload();

  } catch (err) {
    alert("❌ No se pudo registrar el cliente o el vehículo");
    console.error(err);
  }
});


// === EDITAR CLIENTE (PUT) ===
document.getElementById("formEditarCliente").addEventListener("submit", async (e) => {
  e.preventDefault();

  const id = document.getElementById("clienteEditarId").value;

  const clienteActualizado = {
    name: document.getElementById("clienteEditarNombre").value,
    phone: document.getElementById("clienteEditarTelefono").value,
    email: document.getElementById("clienteEditarEmail").value
  };

  try {
    const res = await fetch(`${API_URL}/customers/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(clienteActualizado)
    });

    if (!res.ok) throw new Error("Error al editar cliente");

    alert("✅ Cliente actualizado correctamente");
    modalEditar.style.display = "none";
    location.reload();
  } catch (err) {
    alert("❌ No se pudo actualizar el cliente");
    console.error(err);
  }
});

// === ELIMINAR CLIENTE (DELETE) ===
document.getElementById("formEliminarCliente").addEventListener("submit", async (e) => {
  e.preventDefault();

  const id = document.getElementById("clienteEliminarId").value;

  if (!confirm(`¿Seguro que quieres eliminar el cliente con ID ${id}?`)) return;

  try {
    const res = await fetch(`${API_URL}/customers/${id}`, { method: "DELETE" });

    if (!res.ok) throw new Error("Error al eliminar cliente");

    alert("🗑️ Cliente eliminado correctamente");
    modalEliminar.style.display = "none";
    location.reload();
  } catch (err) {
    alert("❌ No se pudo eliminar el cliente");
    console.error(err);
  }
});
