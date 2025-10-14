import { API_URL } from "./config.js";
let tablaClientes, tablaVehiculos, tablaOrdenes;

document.addEventListener("DOMContentLoaded", function() {
    console.log("📋 Inicializando panel de recepcionista...");
    
    tablaClientes = document.querySelector("#tablaClientes tbody");
    tablaVehiculos = document.querySelector("#tablaVehiculos tbody");
    tablaOrdenes = document.querySelector("#tablaOrdenes tbody");
    
    verificarPermisos();
    
    inicializarNavegacion();
    inicializarCerrarSesion();
    inicializarModales();
    inicializarEventos();
    
    console.log("✅ Panel de recepcionista inicializado correctamente");
});

function verificarPermisos() {
    const role = localStorage.getItem("role");
    const name = localStorage.getItem("username");

    console.log("🔐 Verificando permisos del recepcionista...", { role, name });

    if (!role || role !== "recepcionista") {
        alert("Acceso denegado. Solo los recepcionistas pueden acceder.");
        window.location.href = "index.html";
        return;
    }

    const receptionistNameElement = document.getElementById("receptionistName");
    if (receptionistNameElement) {
        receptionistNameElement.textContent = name || "Recepcionista";
    }
}

function inicializarNavegacion() {
    const links = document.querySelectorAll(".nav-link");
    const sections = document.querySelectorAll(".section");

    function cambiarSeccion(seccionId) {
        console.log(`🔄 Cambiando a sección: ${seccionId}`);
        
        links.forEach(link => link.classList.remove("active"));
        sections.forEach(section => section.classList.remove("active"));

        const linkActivo = document.querySelector(`[data-section="${seccionId}"]`);
        const seccionActiva = document.getElementById(seccionId);

        if (linkActivo && seccionActiva) {
            linkActivo.classList.add("active");
            seccionActiva.classList.add("active");
            console.log(`✅ Sección ${seccionId} activada`);
            
            cargarDatosSeccion(seccionId);
        }
    }

    links.forEach(link => {
        link.addEventListener("click", function(e) {
            e.preventDefault();
            const seccionId = this.getAttribute("data-section");
            cambiarSeccion(seccionId);
        });
    });

    cambiarSeccion("inicio");
}

function cargarDatosSeccion(seccionId) {
    switch(seccionId) {
        case "inicio":
            cargarDashboard();
            break;
        case "clientes":
            cargarClientes();
            break;
        case "vehiculos":
            cargarVehiculos();
            break;
        case "ordenes":
            cargarOrdenes();
            break;
    }
}

function inicializarCerrarSesion() {
    const logoutBtn = document.getElementById("logoutBtn");
    
    if (logoutBtn) {
        logoutBtn.addEventListener("click", function(e) {
            e.preventDefault();
            console.log("🚪 Cerrando sesión...");
            
            if (confirm("¿Estás seguro de que deseas cerrar sesión?")) {
                localStorage.clear();
                window.location.href = "index.html";
            }
        });
    }
}

function inicializarEventos() {
    document.getElementById("btnAplicarFiltros").addEventListener("click", aplicarFiltrosOrdenes);
    document.getElementById("btnLimpiarFiltros").addEventListener("click", limpiarFiltrosOrdenes);

    document.getElementById("btnBuscarCliente").addEventListener("click", buscarClientes);
    document.getElementById("btnBuscarVehiculo").addEventListener("click", buscarVehiculos);

    document.getElementById("buscarCliente").addEventListener("keypress", function(e) {
        if (e.key === 'Enter') buscarClientes();
    });
    document.getElementById("buscarVehiculo").addEventListener("keypress", function(e) {
        if (e.key === 'Enter') buscarVehiculos();
    });

    actualizarFechaActual();
}

function inicializarModales() {
    document.getElementById("btnCrearOrden").addEventListener("click", function() {
        cargarDatosParaOrden();
        document.getElementById("modalCrearOrden").style.display = "block";
    });

    document.getElementById("ordenCerrarCrear").addEventListener("click", function() {
        document.getElementById("modalCrearOrden").style.display = "none";
    });

    document.getElementById("formCrearOrden").addEventListener("submit", function(e) {
        e.preventDefault();
        crearOrden();
    });

    document.getElementById("btnAgregarRepuesto").addEventListener("click", agregarLineaRepuesto);
        document.getElementById("ordenClienteId").addEventListener("change", function() {
        cargarVehiculosPorCliente(this.value);
    });
}

// ===== FUNCIONES PARA CARGAR DATOS =====

async function cargarDashboard() {
    try {
        console.log("🔄 Cargando dashboard...");
        
        const metricas = await obtenerMetricasRecepcion();
        const ordenesRecientes = await obtenerOrdenesRecientes();
        
        actualizarMetricasDashboard(metricas);
        renderizarOrdenesRecientes(ordenesRecientes);
    } catch (error) {
        console.error("❌ Error al cargar dashboard:", error);
        mostrarError("Error al cargar el dashboard");
    }
}

async function cargarClientes() {
    try {
        console.log("🔄 Cargando clientes...");
        const [clientes, vehiculos] = await Promise.all([
            obtenerClientes(),
            obtenerVehiculos()
        ]);

        const clientesConConteo = clientes.map(c => ({
            ...c,
            vehiclesCount: vehiculos.filter(v => v.customerId === c.id).length
        }));

        renderizarTablaClientes(clientesConConteo);
    } catch (error) {
        console.error("❌ Error al cargar clientes:", error);
        tablaClientes.innerHTML = `
            <tr><td colspan="5" class="error">Error al cargar los clientes</td></tr>
        `;
    }
}

async function cargarVehiculos() {
    try {
        console.log("🔄 Cargando vehículos...");
        const [vehiculos, clientes] = await Promise.all([
            obtenerVehiculos(),
            obtenerClientes()
        ]);

        // Agregar nombre del cliente
        const vehiculosConCliente = vehiculos.map(v => {
            const cliente = clientes.find(c => c.id === v.customerId);
            return { ...v, clientName: cliente ? cliente.name : "No asignado" };
        });

        renderizarTablaVehiculos(vehiculosConCliente);
    } catch (error) {
        console.error("❌ Error al cargar vehículos:", error);
        tablaVehiculos.innerHTML = `
            <tr><td colspan="7" class="error">Error al cargar los vehículos</td></tr>
        `;
    }
}

async function cargarOrdenes() {
    try {
        console.log("🔄 Cargando órdenes...");
        
        // Mostrar loading
        tablaOrdenes.innerHTML = `
            <tr>
                <td colspan="8" class="loading">Cargando órdenes...</td>
            </tr>
        `;

        const ordenes = await obtenerOrdenes();
        renderizarTablaOrdenes(ordenes);
    } catch (error) {
        console.error("❌ Error al cargar órdenes:", error);
        tablaOrdenes.innerHTML = `
            <tr>
                <td colspan="8" class="error">Error al cargar las órdenes</td>
            </tr>
        `;
    }
}

// ===== FUNCIONES DE LA API =====

async function obtenerClientes() {
    try {
        const response = await fetch(`${API_URL}/customers/all`);
        if (!response.ok) throw new Error("Error al obtener clientes");
        const clientes = await response.json();
        console.log(`✅ Obtenidos ${clientes.length} clientes`);
        return clientes;
    } catch (error) {
        console.error("Error obteniendo clientes:", error);
        return [];
    }
}

async function obtenerVehiculos() {
    try {
        const response = await fetch(`${API_URL}/vehicles/all`);
        if (!response.ok) throw new Error("Error al obtener vehículos");
        const vehiculos = await response.json();
        console.log(`✅ Obtenidos ${vehiculos.length} vehículos`);
        return vehiculos;
    } catch (error) {
        console.error("Error obteniendo vehículos:", error);
        return [];
    }
}

async function obtenerMecanicos() {
  try {
    const response = await fetch(`${API_URL}/auth/users`);
    if (!response.ok) throw new Error("Error al obtener usuarios");

    const usuarios = await response.json();
    const mecanicos = usuarios.filter(u => 
      u.roles && u.roles.some(r => r.toLowerCase() === "mecanico")
    );

    console.log(`✅ Obtenidos ${mecanicos.length} mecánicos`);
    return mecanicos;
  } catch (error) {
    console.error("❌ Error obteniendo mecánicos:", error);
    return [
      { id: 1, username: "Carlos López", role: "mecanico" },
      { id: 2, username: "Ana Martínez", role: "mecanico" },
      { id: 3, username: "Pedro Gómez", role: "mecanico" }
    ];
  }
}

// 📦 Obtener repuestos desde la API - CORREGIDA PARA LA ESTRUCTURA SparePart
async function obtenerRepuestos() {
    try {
        console.log("🔄 Intentando obtener repuestos desde:", `${API_URL}/spareparts/all`);
        const response = await fetch(`${API_URL}/spareparts/all`);
        
        if (!response.ok) {
            console.error(`❌ Error HTTP: ${response.status} ${response.statusText}`);
            throw new Error(`Error al obtener repuestos: ${response.status}`);
        }

        const repuestos = await response.json();
        console.log("🔍 Repuestos obtenidos (crudos):", repuestos);
        console.log(`✅ Obtenidos ${repuestos.length} repuestos`);
        
        // Mapear correctamente según la estructura SparePart
        const repuestosMapeados = repuestos.map(repuesto => {
            return {
                id: repuesto.id,
                code: repuesto.code || "SIN-CODIGO",
                description: repuesto.description || "Repuesto sin descripción",
                price: parseFloat(repuesto.unitPrice) || 0,
                stock: repuesto.stockQuantity || 0
            };
        });
        
        console.log("🔍 Repuestos mapeados:", repuestosMapeados);
        return repuestosMapeados;

    } catch (error) {
        console.error("❌ Error obteniendo repuestos:", error);
        
        // Datos de respaldo CORREGIDOS - usando description en lugar de name
        return [
            { id: "1", code: "FIL-ACE", description: "Filtro de aceite", price: 15.00, stock: 10 },
            { id: "2", code: "ACE-SYN", description: "Aceite sintético 5W-30", price: 12.50, stock: 25 },
            { id: "3", code: "PAS-FREN", description: "Pastillas de freno delanteras", price: 45.00, stock: 8 },
            { id: "4", code: "BAT-60", description: "Batería 12V 60Ah", price: 120.00, stock: 5 },
            { id: "5", code: "LLAN-185", description: "Llantas 185/65R15", price: 85.00, stock: 12 }
        ];
    }
}

async function obtenerMetricasRecepcion() {
    try {
        const clientes = await obtenerClientes();
        const totalClientes = clientes.length;

        const ordenesHoy = Math.floor(Math.random() * 8) + 3; // 3-10 órdenes
        const ordenesPendientes = Math.floor(Math.random() * 15) + 5; // 5-20 órdenes
        const ordenesProceso = Math.floor(Math.random() * 12) + 3; // 3-15 órdenes
        const ordenesCompletadas = Math.floor(Math.random() * 25) + 10; // 10-35 órdenes

        console.log("📊 Métricas del dashboard (simuladas):", {
            ordenesHoy,
            ordenesPendientes,
            ordenesProceso,
            totalClientes,
            ordenesCompletadas
        });

        return {
            ordenesHoy,
            ordenesPendientes,
            ordenesProceso,
            totalClientes,
            ordenesCompletadas
        };
    } catch (error) {
        console.error("Error obteniendo métricas:", error);
        return {
            ordenesHoy: 0,
            ordenesPendientes: 0,
            ordenesProceso: 0,
            totalClientes: 0,
            ordenesCompletadas: 0
        };
    }
}

// 📋 Obtener órdenes
async function obtenerOrdenes() {
  try {
    const response = await fetch(`${API_URL}/serviceorders/all`);
    if (!response.ok) throw new Error("Error al obtener órdenes");

    const ordenes = await response.json();
    console.log(`✅ Obtenidas ${ordenes.length} órdenes desde la API`);

    // Enriquecer las órdenes con datos adicionales
    return await enriquecerOrdenesConDatosCompletos(ordenes);

  } catch (error) {
    console.error("❌ Error obteniendo órdenes:", error);
    mostrarError("No se pudieron cargar las órdenes");
    return [];
  }
}

async function enriquecerOrdenesConDatosCompletos(ordenes) {
  try {
    // Obtener datos adicionales necesarios
    const [vehiculos, clientes, mecanicos, repuestos] = await Promise.all([
      obtenerVehiculos(),
      obtenerClientes(),
      obtenerMecanicos(),
      obtenerRepuestos()
    ]);

    console.log("🔄 Enriqueciendo órdenes con datos adicionales...");

    return ordenes.map(orden => {
      // Buscar vehículo relacionado
      const vehiculo = vehiculos.find(v => v.id === orden.vehicleId);
      
      // Buscar cliente a través del vehículo
      const cliente = vehiculo ? clientes.find(c => c.id === vehiculo.customerId) : null;
      
      // Buscar mecánico
      const mecanico = mecanicos.find(m => m.id === orden.userMemberId);

      // Buscar detalles de repuestos para esta orden
      const detallesRepuestos = orden.orderDetails || [];

      // Calcular costo total
      const totalRepuestos = detallesRepuestos.reduce((total, detalle) => {
        const repuesto = repuestos.find(r => r.id === detalle.sparePartId);
        return total + (detalle.quantity * detalle.unitCost);
      }, 0);

      // Mapear estados
      const estados = {
        1: "Pendiente",
        2: "En Proceso", 
        3: "Completado",
      };

      // Mapear tipos de servicio
      const tiposServicio = {
        1: "Mantenimiento preventivo",
        2: "Reparación",
        3: "Diagnóstico"
      };

      return {
        id: orden.id,
        vehicleId: orden.vehicleId,
        vehicle: vehiculo ? {
          brand: vehiculo.brand,
          model: vehiculo.model,
          year: vehiculo.year,
          vin: vehiculo.vin,
          mileage: vehiculo.mileage,
          licensePlate: vehiculo.licensePlate
        } : { 
          brand: "Vehículo no encontrado", 
          model: "",
          id: orden.vehicleId 
        },
        client: cliente ? {
          name: cliente.name,
          phone: cliente.phone,
          email: cliente.email,
          id: cliente.id
        } : { 
          name: "Cliente no encontrado",
          id: vehiculo?.customerId 
        },
        serviceType: tiposServicio[orden.serviceType] || `Servicio #${orden.serviceType}`,
        serviceTypeId: orden.serviceType,
        orderStatus: estados[orden.orderStatus] || `Estado #${orden.orderStatus}`,
        orderStatusId: orden.orderStatus,
        entryDate: orden.entryDate,
        estimatedDeliveryDate: orden.estimatedDeliveryDate,
        mechanic: mecanico ? {
          name: mecanico.username || mecanico.name,
          id: mecanico.id
        } : { 
          name: "Mecánico no asignado",
          id: orden.userMemberId 
        },
        totalCost: totalRepuestos,
        notes: orden.notes || "",
        orderDetails: detallesRepuestos,
        // Datos originales para depuración
        _original: orden
      };
    });

  } catch (error) {
    console.error("❌ Error enriqueciendo órdenes:", error);
    // Si hay error, devolver las órdenes básicas
    return ordenes.map(orden => ({
      id: orden.id,
      vehicle: { brand: "Error cargando", model: "" },
      client: { name: "Error cargando" },
      serviceType: `Servicio ${orden.serviceType}`,
      orderStatus: `Estado ${orden.orderStatus}`,
      entryDate: orden.entryDate,
      estimatedDeliveryDate: orden.estimatedDeliveryDate,
      mechanic: { name: "Error cargando" },
      _original: orden // Mantener datos originales para debug
    }));
  }
}

async function obtenerOrdenesRecientes() {
    const ordenesRecientes = [
        {
            id: "123e4567-e89b-12d3-a456-426614174000",
            vehicle: { brand: "Toyota", model: "Corolla" },
            client: { name: "Juan Pérez" },
            orderStatus: "Pendiente",
            entryDate: new Date().toISOString()
        },
        {
            id: "123e4567-e89b-12d3-a456-426614174001",
            vehicle: { brand: "Honda", model: "Civic" },
            client: { name: "María García" },
            orderStatus: "EnProceso",
            entryDate: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000).toISOString()
        },
        {
            id: "123e4567-e89b-12d3-a456-426614174003",
            vehicle: { brand: "Nissan", model: "Sentra" },
            client: { name: "Carlos Ruiz" },
            orderStatus: "Completado",
            entryDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString()
        }
    ];

    console.log("🕒 Órdenes recientes simuladas cargadas");
    return ordenesRecientes;
}

async function buscarClientes() {
    const termino = document.getElementById("buscarCliente").value.trim();
    
    try {
        let clientes;
        if (termino) {
            // Buscar clientes por término
            const response = await fetch(`${API_URL}/customers/search?q=${encodeURIComponent(termino)}`);
            if (!response.ok) throw new Error("Error en búsqueda de clientes");
            clientes = await response.json();
        } else {
            // Cargar todos los clientes
            clientes = await obtenerClientes();
        }
        
        renderizarTablaClientes(clientes);
    } catch (error) {
        console.error("Error buscando clientes:", error);
        mostrarError("Error al buscar clientes");
    }
}

async function buscarVehiculos() {
    const termino = document.getElementById("buscarVehiculo").value.trim();
    
    try {
        let vehiculos;
        if (termino) {
            const response = await fetch(`${API_URL}/vehicles/search?q=${encodeURIComponent(termino)}`);
            if (!response.ok) throw new Error("Error en búsqueda de vehículos");
            vehiculos = await response.json();
        } else {
            vehiculos = await obtenerVehiculos();
        }
        
        renderizarTablaVehiculos(vehiculos);
    } catch (error) {
        console.error("Error buscando vehículos:", error);
        mostrarError("Error al buscar vehículos");
    }
}

// ===== RENDERIZADO DE DATOS =====

function actualizarMetricasDashboard(metricas) {
    document.getElementById("totalOrdenesDia").textContent = metricas.ordenesHoy || 0;
    document.getElementById("totalPendientes").textContent = metricas.ordenesPendientes || 0;
    document.getElementById("totalProceso").textContent = metricas.ordenesProceso || 0;
    document.getElementById("totalClientes").textContent = metricas.totalClientes || 0;
    document.getElementById("ordenesPendientes").textContent = metricas.ordenesPendientes || 0;
    document.getElementById("ordenesProceso").textContent = metricas.ordenesProceso || 0;
    document.getElementById("ordenesCompletadas").textContent = metricas.ordenesCompletadas || 0;
}

function renderizarTablaClientes(clientes) {
    if (!tablaClientes) return;
    
    if (!clientes || clientes.length === 0) {
        tablaClientes.innerHTML = `
            <tr>
                <td colspan="5" class="no-data">No se encontraron clientes</td>
            </tr>
        `;
        return;
    }

    tablaClientes.innerHTML = clientes.map(cliente => `
        <tr>
            <td>${cliente.id}</td>
            <td>${cliente.name || 'Sin nombre'}</td>
            <td>${cliente.phone || '—'}</td>
            <td>${cliente.email || '—'}</td>
            <td>${cliente.vehiclesCount || 0}</td>
        </tr>
    `).join('');
}

function renderizarTablaVehiculos(vehiculos) {
    if (!tablaVehiculos) return;
    
    if (!vehiculos || vehiculos.length === 0) {
        tablaVehiculos.innerHTML = `
            <tr>
                <td colspan="7" class="no-data">No se encontraron vehículos</td>
            </tr>
        `;
        return;
    }

    tablaVehiculos.innerHTML = vehiculos.map(vehiculo => `
        <tr>
            <td>${vehiculo.id}</td>
            <td>${vehiculo.vin || vehiculo.placa || '—'}</td>
            <td>${vehiculo.brand || '—'}</td>
            <td>${vehiculo.model || '—'}</td>
            <td>${vehiculo.year || '—'}</td>
            <td>${vehiculo.mileage || '—'}</td>
            <td>${vehiculo.clientName || 'No asignado'}</td>
        </tr>
    `).join('');
}

function renderizarTablaOrdenes(ordenes) {
  if (!tablaOrdenes) return;
  
  if (!ordenes || ordenes.length === 0) {
    tablaOrdenes.innerHTML = `
      <tr>
        <td colspan="8" class="no-data"> <!-- Mantener 8 columnas -->
          <div style="text-align: center; padding: 20px;">
            <i style="font-size: 48px; color: #ccc; margin-bottom: 10px;">📋</i>
            <p>No se encontraron órdenes de servicio</p>
          </div>
        </td>
      </tr>
    `;
    return;
  }

  tablaOrdenes.innerHTML = ordenes.map(orden => {
    // Manejar casos donde los datos puedan ser undefined
    const vehicleBrand = orden.vehicle?.brand || 'N/A';
    const vehicleModel = orden.vehicle?.model || '';
    const clientName = orden.client?.name || 'N/A';
    const serviceType = orden.serviceType || 'N/A';
    const orderStatus = orden.orderStatus || 'Pendiente';
    const mechanicName = orden.mechanic?.name || 'No asignado';
    
    const entryDate = orden.entryDate ? new Date(orden.entryDate).toLocaleDateString() : 'N/A';
    const estimatedDate = orden.estimatedDeliveryDate ? new Date(orden.estimatedDeliveryDate).toLocaleDateString() : 'N/A';

    return `
      <tr>
        <td>
          <strong title="${orden.id}">#${orden.id}</strong> <!-- ✅ ID COMPLETO -->
        </td>
        <td>
          <div>
            <strong>${vehicleBrand} ${vehicleModel}</strong>
            ${orden.vehicle?.year ? `<br><small>Año: ${orden.vehicle.year}</small>` : ''}
            ${orden.vehicle?.vin ? `<br><small>VIN: ${orden.vehicle.vin}</small>` : ''}
          </div>
        </td>
        <td>
          <div>
            <strong>${clientName}</strong>
            ${orden.client?.phone ? `<br><small>📞 ${orden.client.phone}</small>` : ''}
            ${orden.client?.email ? `<br><small>✉️ ${orden.client.email}</small>` : ''}
          </div>
        </td>
        <td>
          <strong>${serviceType}</strong>
          ${orden.serviceTypeId ? `<br><small>ID: ${orden.serviceTypeId}</small>` : ''}
        </td>
        <td>
          <div style="font-size: 0.9em;">
            <div><strong>Entrada:</strong><br>${entryDate}</div>
            ${estimatedDate !== 'N/A' ? `<div style="margin-top: 4px;"><strong>Estimada:</strong><br>${estimatedDate}</div>` : ''}
          </div>
        </td>
        <td>
          <span class="badge estado-${(orderStatus || 'pendiente').toLowerCase().replace(' ', '-')}">
            ${orderStatus}
          </span>
          ${orden.orderStatusId ? `<br><small>ID: ${orden.orderStatusId}</small>` : ''}
        </td>
        <td>
          <strong>${mechanicName}</strong>
          ${orden.mechanic?.id ? `<br><small>ID: ${orden.mechanic.id}</small>` : ''}
        </td>
        <td style="text-align: right; font-weight: bold;">
          ${orden.totalCost && orden.totalCost > 0 ? `$${orden.totalCost.toFixed(2)}` : '<span style="color: #999;">--</span>'}
        </td>
      </tr>
    `;
  }).join('');

  console.log("✅ Tabla de órdenes renderizada con", ordenes.length, "órdenes");
}

function renderizarOrdenesRecientes(ordenes) {
    const container = document.getElementById("recentOrders");
    if (!container) return;

    if (!ordenes || ordenes.length === 0) {
        container.innerHTML = '<div class="no-data">No hay órdenes recientes</div>';
        return;
    }

    container.innerHTML = ordenes.map(orden => `
        <div class="recent-order-item">
            <div class="recent-order-header">
                <span class="order-id">#${orden.id.substring(0, 8)}</span>
                <span class="badge estado-${orden.orderStatus?.toLowerCase() || 'pendiente'}">${orden.orderStatus || 'Pendiente'}</span>
            </div>
            <div class="recent-order-details">
                <span>${orden.vehicle?.brand || ''} ${orden.vehicle?.model || ''}</span>
                <span>${orden.client?.name || ''}</span>
            </div>
            <div class="recent-order-date">
                ${new Date(orden.entryDate).toLocaleDateString()}
            </div>
        </div>
    `).join('');
}

// ===== FUNCIONALIDADES DEL MODAL DE ORDEN =====

async function cargarDatosParaOrden() {
    await cargarClientesParaOrden();
    await cargarMecanicosParaOrden();
    await cargarRepuestosParaOrden();
    
    const hoy = new Date().toISOString().split('T')[0];
    document.getElementById("ordenFechaEntrega").min = hoy;
}

async function cargarClientesParaOrden() {
    const select = document.getElementById("ordenClienteId");
    if (!select) return;
    
    try {
        const clientes = await obtenerClientes();
        select.innerHTML = '<option value="">Seleccionar cliente...</option>';
        
        clientes.forEach(cliente => {
            const option = document.createElement("option");
            option.value = cliente.id;
            option.textContent = cliente.name;
            select.appendChild(option);
        });
    } catch (error) {
        console.error("Error cargando clientes para orden:", error);
    }
}

async function cargarMecanicosParaOrden() {
  const select = document.getElementById("ordenMecanicoId");
  if (!select) return;

  try {
    const mecanicos = await obtenerMecanicos();

    select.innerHTML = '<option value="">Seleccionar mecánico...</option>';

    mecanicos.forEach(mecanico => {
      const option = document.createElement("option");
      option.value = mecanico.id;
      option.textContent = mecanico.username || mecanico.name || "Sin nombre";
      select.appendChild(option);
    });

  } catch (error) {
    console.error("❌ Error cargando mecánicos para orden:", error);
  }
}

async function cargarVehiculosPorCliente(clienteId) {
    const select = document.getElementById("ordenVehiculoId");
    const infoDiv = document.getElementById("vehicleInfo");
    
    if (!select || !infoDiv) return;
    
    select.innerHTML = '<option value="">Seleccionar vehículo...</option>';
    infoDiv.innerHTML = '';
    
    if (!clienteId) return;
    
    try {
        const vehiculos = await obtenerVehiculos();
        const vehiculosCliente = vehiculos.filter(v => v.customerId == clienteId);
        
        vehiculosCliente.forEach(vehiculo => {
            const option = document.createElement("option");
            option.value = vehiculo.id;
            option.textContent = `${vehiculo.brand} ${vehiculo.model} - ${vehiculo.vin || vehiculo.placa}`;
            select.appendChild(option);
        });
        
        select.addEventListener("change", function() {
            const vehiculoSeleccionado = vehiculosCliente.find(v => v.id === this.value);
            if (vehiculoSeleccionado) {
                infoDiv.innerHTML = `
                    <div class="vehicle-details">
                        <p><strong>Marca:</strong> ${vehiculoSeleccionado.brand}</p>
                        <p><strong>Modelo:</strong> ${vehiculoSeleccionado.model}</p>
                        <p><strong>Año:</strong> ${vehiculoSeleccionado.year}</p>
                        <p><strong>Kilometraje:</strong> ${vehiculoSeleccionado.mileage} km</p>
                    </div>
                `;
            } else {
                infoDiv.innerHTML = '';
            }
        });
    } catch (error) {
        console.error("Error cargando vehículos por cliente:", error);
    }
}

async function crearOrden() {
  const clienteId = document.getElementById("ordenClienteId").value;
  const vehiculoId = document.getElementById("ordenVehiculoId").value;
  const mecanicoId = document.getElementById("ordenMecanicoId").value;
  const tipoServicio = document.getElementById("ordenTipoServicio").value;
  const fechaEntrega = document.getElementById("ordenFechaEntrega").value;

  if (!clienteId || !vehiculoId || !mecanicoId || !tipoServicio || !fechaEntrega) {
    mostrarError("Por favor complete todos los campos requeridos");
    return;
  }

  try {
    // Preparar datos de la orden
    const ordenData = {
      vehicleId: vehiculoId,
      serviceType: parseInt(tipoServicio),
      userMemberId: parseInt(mecanicoId),
      entryDate: new Date().toISOString().split('T')[0],
      estimatedDeliveryDate: fechaEntrega,
      orderStatus: 1 // Pendiente
    };

    console.log("📦 Datos de orden a enviar:", ordenData);

    // 1️⃣ Crear la orden principal
    const response = await fetch(`${API_URL}/serviceorders`, {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      },
      body: JSON.stringify(ordenData)
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.error("❌ Error del servidor:", errorText);
      throw new Error(`Error ${response.status}: ${errorText}`);
    }

    const nuevaOrden = await response.json();
    console.log("✅ Orden creada:", nuevaOrden);

    // 2️⃣ Crear detalles de repuestos CORREGIDO
    const repuestos = document.querySelectorAll(".repuesto-item");
    let detallesCreados = 0;
    let erroresRepuestos = [];

    for (const rep of repuestos) {
      const selectElement = rep.querySelector(".repuesto-select");
      const selectedOption = selectElement.selectedOptions[0];
      const sparePartId = selectElement.value;
      const quantity = parseInt(rep.querySelector(".repuesto-cantidad").value) || 0;
      
      // ✅ CORRECCIÓN: Obtener el precio del dataset del option seleccionado
      const unitCost = parseFloat(selectedOption?.dataset.precio) || 0;

      console.log("🔍 Validando repuesto:", {
        sparePartId,
        quantity,
        unitCost,
        selectedOption: selectedOption?.textContent
      });

      // Validaciones más estrictas
      if (!sparePartId) {
        console.warn("⚠️ Repuesto sin ID seleccionado, omitiendo...");
        continue;
      }

      if (quantity <= 0) {
        console.warn("⚠️ Cantidad inválida:", quantity);
        erroresRepuestos.push(`Cantidad inválida para repuesto: ${selectedOption?.textContent}`);
        continue;
      }

      if (unitCost <= 0) {
        console.warn("⚠️ Precio unitario inválido:", unitCost);
        erroresRepuestos.push(`Precio inválido para repuesto: ${selectedOption?.textContent}`);
        continue;
      }

      // Verificar stock si está disponible
      const stockDisponible = parseInt(selectedOption?.dataset.stock) || 0;
      if (stockDisponible > 0 && quantity > stockDisponible) {
        console.warn("⚠️ Stock insuficiente:", { quantity, stockDisponible });
        erroresRepuestos.push(`Stock insuficiente para: ${selectedOption?.textContent} (Solicitado: ${quantity}, Disponible: ${stockDisponible})`);
        continue;
      }

      const detailData = {
        serviceOrderId: nuevaOrden.id,
        sparePartId: sparePartId,
        quantity: quantity,
        unitCost: unitCost
      };

      console.log("📦 Creando detalle de repuesto:", detailData);

      try {
        const detailResponse = await fetch(`${API_URL}/orderdetails`, {
          method: 'POST',
          headers: { 
            'Content-Type': 'application/json',
            'Accept': 'application/json'
          },
          body: JSON.stringify(detailData)
        });

        if (detailResponse.ok) {
          detallesCreados++;
          console.log(`✅ Detalle ${detallesCreados} creado exitosamente`);
        } else {
          const errorDetail = await detailResponse.text();
          console.error("❌ Error creando detalle:", errorDetail);
          erroresRepuestos.push(`Error al agregar: ${selectedOption?.textContent}`);
        }
      } catch (error) {
        console.error("❌ Error en petición de detalle:", error);
        erroresRepuestos.push(`Error de conexión con: ${selectedOption?.textContent}`);
      }
    }

    // Mostrar resultado
    let mensaje = `Orden #${nuevaOrden.id.substring(0, 8)} creada exitosamente.`;
    
    if (detallesCreados > 0) {
      mensaje += ` ${detallesCreados} repuesto(s) agregado(s).`;
    } else {
      mensaje += " No se agregaron repuestos.";
    }

    if (erroresRepuestos.length > 0) {
      mensaje += `\n\nErrores:\n• ${erroresRepuestos.join('\n• ')}`;
      mostrarError(mensaje);
    } else {
      mostrarExito(mensaje);
    }

    // Limpiar y cerrar modal
    document.getElementById("modalCrearOrden").style.display = "none";
    document.getElementById("formCrearOrden").reset();
    
    // Limpiar repuestos
    const repuestosContainer = document.querySelector(".repuestos-container");
    const repuestosExistentes = repuestosContainer.querySelectorAll(".repuesto-item:not(#repuestoTemplate)");
    repuestosExistentes.forEach(rep => rep.remove());

    // Recargar datos
    cargarOrdenes();
    cargarDashboard();

  } catch (error) {
    console.error("❌ Error creando orden:", error);
    mostrarError(`Error al crear la orden: ${error.message}`);
  }
}

// ===== FUNCIONES AUXILIARES =====

function actualizarFechaActual() {
    const ahora = new Date();
    const opciones = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    const fechaElement = document.getElementById("fechaActual");
    if (fechaElement) {
        fechaElement.textContent = ahora.toLocaleDateString('es-ES', opciones);
    }
}

function aplicarFiltrosOrdenes() {
    const estado = document.getElementById("filterEstado").value;
    const fecha = document.getElementById("filterFecha").value;
    
    console.log(`Aplicando filtros: Estado=${estado}, Fecha=${fecha}`);
    cargarOrdenes();
}

function limpiarFiltrosOrdenes() {
    document.getElementById("filterEstado").value = "all";
    document.getElementById("filterFecha").value = "";
    cargarOrdenes();
}

function mostrarError(mensaje) {
    alert(`Error: ${mensaje}`);
}

function mostrarExito(mensaje) {
    alert(`Éxito: ${mensaje}`);
}

// ===== FUNCIONES PARA REPUESTOS =====

// 📦 Agregar línea de repuesto
function agregarLineaRepuesto() {
    const container = document.querySelector(".repuestos-container");
    const template = document.getElementById("repuestoTemplate");
    
    if (!template || !container) {
        console.error("❌ No se encontró el template de repuestos o el contenedor");
        mostrarError("Error al agregar línea de repuesto");
        return;
    }
    
    try {
        const nuevaLinea = template.cloneNode(true);
        nuevaLinea.id = ""; // Remover el ID para evitar duplicados
        nuevaLinea.style.display = "flex"; // Mostrar la línea
        
        // Configurar eventos
        const quitarBtn = nuevaLinea.querySelector(".quitar-repuesto");
        const selectElement = nuevaLinea.querySelector(".repuesto-select");
        const cantidadInput = nuevaLinea.querySelector(".repuesto-cantidad");
        
        if (quitarBtn && selectElement && cantidadInput) {
            quitarBtn.addEventListener("click", function() {
                nuevaLinea.remove();
                calcularTotalOrden();
            });
            
            selectElement.addEventListener("change", function() {
                actualizarPrecioRepuesto(this);
            });
            
            cantidadInput.addEventListener("change", function() {
                actualizarSubtotalRepuesto(this);
            });
            
            cantidadInput.addEventListener("input", function() {
                actualizarSubtotalRepuesto(this);
            });
            
            // Cargar repuestos en este select específico
            cargarRepuestosParaSelect(selectElement);
            
            container.appendChild(nuevaLinea);
            console.log("✅ Nueva línea de repuesto agregada");
        } else {
            console.error("❌ Elementos del template no encontrados");
        }
        
    } catch (error) {
        console.error("❌ Error al agregar línea de repuesto:", error);
        mostrarError("Error al agregar repuesto");
    }
}

async function cargarRepuestosParaOrden() {
    const container = document.querySelector(".repuestos-container");
    if (!container) return;
    
    // Limpiar repuestos existentes (excepto el template)
    const repuestosExistentes = container.querySelectorAll(".repuesto-item:not(#repuestoTemplate)");
    repuestosExistentes.forEach(repuesto => {
        repuesto.remove();
    });
    
    // Agregar una línea inicial
    agregarLineaRepuesto();
}

async function cargarRepuestosParaSelect(selectElement) {
    try {
        console.log("🔄 Cargando repuestos para select...");
        const repuestos = await obtenerRepuestos();
        
        // Limpiar el select
        selectElement.innerHTML = '<option value="">Seleccionar repuesto...</option>';
        
        if (!repuestos || repuestos.length === 0) {
            console.warn("⚠️ No se encontraron repuestos");
            selectElement.innerHTML += '<option value="" disabled>No hay repuestos disponibles</option>';
            return;
        }
        
        // Agregar repuestos al select
        repuestos.forEach(repuesto => {
            const option = document.createElement("option");
            option.value = repuesto.id;
            option.textContent = `${repuesto.code} - ${repuesto.description} - $${repuesto.price.toFixed(2)}`;
            option.dataset.precio = repuesto.price;
            option.dataset.stock = repuesto.stock;
            option.dataset.code = repuesto.code;
            option.dataset.description = repuesto.description; // CORREGIDO: description en lugar de name
            selectElement.appendChild(option);
        });
        
        console.log(`✅ ${repuestos.length} repuestos cargados en el select`);

    } catch (error) {
        console.error("❌ Error cargando repuestos en select:", error);
        // Datos de respaldo CORREGIDOS - usando description en lugar de name
        selectElement.innerHTML = `
            <option value="">Seleccionar repuesto...</option>
            <option value="1" data-precio="15.00" data-stock="10" data-code="FIL-ACE" data-description="Filtro de aceite">FIL-ACE - Filtro de aceite - $15.00</option>
            <option value="2" data-precio="12.50" data-stock="25" data-code="ACE-SYN" data-description="Aceite sintético">ACE-SYN - Aceite sintético - $12.50</option>
            <option value="3" data-precio="45.00" data-stock="8" data-code="PAS-FREN" data-description="Pastillas de freno">PAS-FREN - Pastillas de freno - $45.00</option>
        `;
    }
}

function actualizarPrecioRepuesto(selectElement) {
    const repuestoItem = selectElement.closest(".repuesto-item");
    if (!repuestoItem) {
        console.warn("⚠️ No se encontró el contenedor del repuesto");
        return;
    }
    
    const precioElement = repuestoItem.querySelector(".repuesto-precio");
    const cantidadInput = repuestoItem.querySelector(".repuesto-cantidad");
    const stockElement = repuestoItem.querySelector(".repuesto-stock");
    const codeElement = repuestoItem.querySelector(".repuesto-code");
    
    if (!precioElement || !cantidadInput) {
        console.error("❌ Elementos del repuesto no encontrados");
        return;
    }
    
    const selectedOption = selectElement.selectedOptions[0];
    if (!selectedOption || !selectedOption.value) {
        precioElement.textContent = "$0.00";
        if (stockElement) stockElement.textContent = "Stock: 0";
        if (codeElement) codeElement.textContent = "";
        return;
    }
    
    const precio = parseFloat(selectedOption.dataset.precio) || 0;
    const stock = parseInt(selectedOption.dataset.stock) || 0;
    const code = selectedOption.dataset.code || "";
    const description = selectedOption.dataset.description || ""; // CORREGIDO: description en lugar de name
    
    precioElement.textContent = `$${precio.toFixed(2)}`;
    
    if (stockElement) {
        stockElement.textContent = `Stock: ${stock}`;
        stockElement.style.color = stock > 10 ? "green" : stock > 0 ? "orange" : "red";
        stockElement.style.fontWeight = stock < 5 ? "bold" : "normal";
    }
    
    if (codeElement) {
        codeElement.textContent = code;
    }
    
    // Actualizar tooltip con información completa
    selectElement.title = `${code} - ${description} | Stock: ${stock} | Precio: $${precio.toFixed(2)}`;
    
    // Resetear cantidad si excede el stock
    const cantidadActual = parseInt(cantidadInput.value) || 0;
    if (cantidadActual > stock) {
        cantidadInput.value = Math.min(cantidadActual, stock);
        if (cantidadActual > 0) {
            mostrarError(`Cantidad ajustada al stock disponible: ${stock}`);
        }
    }
    
    cantidadInput.max = stock; // Establecer máximo según stock
    
    actualizarSubtotalRepuesto(cantidadInput);
}

function actualizarSubtotalRepuesto(cantidadInput) {
    const repuestoItem = cantidadInput.closest(".repuesto-item");
    if (!repuestoItem) return;
    
    const precioElement = repuestoItem.querySelector(".repuesto-precio");
    const subtotalElement = repuestoItem.querySelector(".repuesto-subtotal");
    
    const precio = parseFloat(precioElement.textContent.replace('$', '')) || 0;
    const cantidad = parseInt(cantidadInput.value) || 0;
    const subtotal = precio * cantidad;
    
    subtotalElement.textContent = `$${subtotal.toFixed(2)}`;
    
    calcularTotalOrden();
}

function calcularTotalOrden() {
    const subtotales = document.querySelectorAll(".repuesto-subtotal");
    let total = 0;
    
    subtotales.forEach(subtotalElement => {
        const valor = parseFloat(subtotalElement.textContent.replace('$', '')) || 0;
        total += valor;
    });
    
    const totalElement = document.getElementById("ordenTotal");
    if (totalElement) {
        totalElement.textContent = total.toFixed(2);
    }
}

// Función de depuración para repuestos
async function debugRepuestos() {
    console.log("=== DEBUG REPUESTOS ===");
    try {
        const repuestos = await obtenerRepuestos();
        console.log("Repuestos obtenidos:", repuestos);
        
        const selects = document.querySelectorAll(".repuesto-select");
        console.log("Selects de repuestos encontrados:", selects.length);
        
        selects.forEach((select, index) => {
            console.log(`Select ${index + 1}:`, {
                id: select.id,
                options: select.options.length,
                selectedValue: select.value,
                selectedText: select.selectedOptions[0]?.textContent
            });
            
            // Mostrar todas las opciones
            console.log("Opciones disponibles:");
            Array.from(select.options).forEach(option => {
                console.log(`  - ${option.value}: ${option.textContent}`, {
                    precio: option.dataset.precio,
                    stock: option.dataset.stock,
                    code: option.dataset.code,
                    description: option.dataset.description
                });
            });
        });
    } catch (error) {
        console.error("Error en debug:", error);
    }
}