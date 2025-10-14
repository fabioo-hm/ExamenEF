import { API_URL } from "./config.js";
let tablaOrdenes, tablaFacturas, tablaTodasFacturas;
let mecanicoId;

document.addEventListener("DOMContentLoaded", function() {
    console.log("🔧 Inicializando panel de mecánico...");
    
    tablaOrdenes = document.querySelector("#tablaOrdenes tbody");
    tablaFacturas = document.querySelector("#tablaFacturas tbody");
    tablaTodasFacturas = document.querySelector("#tablaTodasFacturas tbody");
    
    verificarPermisosMecanico();
    inicializarNavegacion();
    inicializarCerrarSesion();
    inicializarModales();
    inicializarEventos();
    
    console.log("✅ Panel de mecánico inicializado correctamente");
});

function verificarPermisosMecanico() {
    const role = localStorage.getItem("role");
    const name = localStorage.getItem("username");
    
    console.log("🔍 Todos los items del localStorage:", { ...localStorage });
    
    mecanicoId = localStorage.getItem("userId") || 
                  localStorage.getItem("id") || 
                  localStorage.getItem("userid") ||
                  localStorage.getItem("user_id");

    console.log("🔐 Verificando permisos del mecánico...", { 
        role, 
        name, 
        mecanicoId,
        localStorage: { ...localStorage } 
    });

    if (!role || role !== "mecanico") {
        alert("Acceso denegado. Solo los mecánicos pueden acceder.");
        window.location.href = "index.html";
        return;
    }

    if (!mecanicoId) {
        console.warn("⚠️ No se encontró userId en localStorage, intentando obtener de la API...");
        obtenerIdMecanicoDesdeAPI(name);
        return;
    }

    const mechanicNameElement = document.getElementById("mechanicName");
    if (mechanicNameElement) {
        mechanicNameElement.textContent = name || "Mecánico";
    }
}


async function obtenerIdMecanicoDesdeAPI(username) {
    try {
        console.log(`🔄 Buscando ID del mecánico por username: ${username}`);
        
        const response = await fetch(`${API_URL}/auth/users`);
        if (!response.ok) throw new Error("Error al obtener usuarios");

        const usuarios = await response.json();
        console.log("🔍 Usuarios obtenidos:", usuarios);
        
        const usuarioMecanico = usuarios.find(u => 
            u.username === username && 
            u.roles && 
            u.roles.some(r => r.toLowerCase() === "mecanico")
        );

        if (usuarioMecanico) {
            mecanicoId = usuarioMecanico.id;
            console.log(`✅ ID del mecánico encontrado: ${mecanicoId}`);
            
            localStorage.setItem("userId", mecanicoId);
            
            const mechanicNameElement = document.getElementById("mechanicName");
            if (mechanicNameElement) {
                mechanicNameElement.textContent = username || "Mecánico";
            }
            
            cargarDatosSeccion("ordenes");
            
        } else {
            console.error("❌ No se pudo encontrar el ID del mecánico");
            mostrarError("Error: No se pudo identificar al mecánico. Contacte al administrador.");
        }

    } catch (error) {
        console.error("❌ Error obteniendo ID del mecánico:", error);
        mostrarError("Error al cargar la información del mecánico");
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

    cambiarSeccion("ordenes");
}

function cargarDatosSeccion(seccionId) {
    switch(seccionId) {
        case "ordenes":
            cargarOrdenesMecanico();
            break;
        case "facturas":
            cargarOrdenesParaFactura();
            break;
        case "todas-facturas":
            cargarTodasLasFacturas();
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
    actualizarFechaActual();
}

function inicializarModales() {
    
    document.getElementById("cerrarActualizarOrden").addEventListener("click", function() {
        document.getElementById("modalActualizarOrden").style.display = "none";
    });

    document.getElementById("formActualizarOrden").addEventListener("submit", function(e) {
        e.preventDefault();
        actualizarEstadoOrden();
    });

    
    document.getElementById("cerrarGenerarFactura").addEventListener("click", function() {
        document.getElementById("modalGenerarFactura").style.display = "none";
    });

    document.getElementById("formGenerarFactura").addEventListener("submit", function(e) {
        e.preventDefault();
        crearFactura();
    });

    
    document.getElementById("cerrarDetalleFactura").addEventListener("click", function() {
        document.getElementById("modalDetalleFactura").style.display = "none";
    });
}



async function cargarOrdenesMecanico() {
    try {
        console.log("🔄 Cargando órdenes del mecánico...");
        
        tablaOrdenes.innerHTML = `
            <tr>
                <td colspan="8" class="loading">Cargando órdenes asignadas...</td>
            </tr>
        `;

        const ordenes = await obtenerOrdenesPorMecanico();
        actualizarMetricasOrdenes(ordenes);
        renderizarTablaOrdenes(ordenes);
    } catch (error) {
        console.error("❌ Error al cargar órdenes del mecánico:", error);
        tablaOrdenes.innerHTML = `
            <tr>
                <td colspan="8" class="error">Error al cargar las órdenes</td>
            </tr>
        `;
    }
}

async function cargarOrdenesParaFactura() {
    try {
        console.log("🔄 Cargando órdenes para facturación...");
        
        tablaFacturas.innerHTML = `
            <tr>
                <td colspan="7" class="loading">Cargando órdenes para facturación...</td>
            </tr>
        `;

        const ordenes = await obtenerOrdenesCompletadasSinFactura();
        renderizarTablaFacturas(ordenes);
    } catch (error) {
        console.error("❌ Error al cargar órdenes para factura:", error);
        tablaFacturas.innerHTML = `
            <tr>
                <td colspan="7" class="error">Error al cargar las órdenes para facturación</td>
            </tr>
        `;
    }
}

async function cargarTodasLasFacturas() {
    try {
        console.log("🔄 Cargando todas las facturas...");
        
        if (tablaTodasFacturas) {
            tablaTodasFacturas.innerHTML = `
                <tr>
                    <td colspan="8" class="loading">Cargando todas las facturas...</td>
                </tr>
            `;
        }

        const facturas = await obtenerTodasLasFacturas();
        renderizarTablaTodasFacturas(facturas);
    } catch (error) {
        console.error("❌ Error al cargar todas las facturas:", error);
        if (tablaTodasFacturas) {
            tablaTodasFacturas.innerHTML = `
                <tr>
                    <td colspan="8" class="error">Error al cargar las facturas</td>
                </tr>
            `;
        }
    }
}



async function obtenerOrdenesPorMecanico() {
    try {
        if (!mecanicoId) {
            console.warn("⚠️ Esperando por mecanicoId...");
            return [];
        }

        console.log(`🔄 Obteniendo órdenes para el mecánico ID: ${mecanicoId}`);
        
        const response = await fetch(`${API_URL}/serviceorders/all`);
        if (!response.ok) throw new Error("Error al obtener órdenes");

        const ordenes = await response.json();
        
        const ordenesMecanico = ordenes.filter(orden => {
            const coincide = parseInt(orden.userMemberId) === parseInt(mecanicoId);
            return coincide;
        });

        console.log(`✅ Obtenidas ${ordenesMecanico.length} órdenes del mecánico`);
        return await enriquecerOrdenesConDatosCompletos(ordenesMecanico);

    } catch (error) {
        console.error("❌ Error obteniendo órdenes del mecánico:", error);
        return [];
    }
}

async function obtenerOrdenesCompletadasSinFactura() {
    try {
        console.log("🔄 Obteniendo órdenes completadas sin factura...");
        
        const ordenesMecanico = await obtenerOrdenesPorMecanico();
        const ordenesCompletadas = ordenesMecanico.filter(orden => 
            orden.orderStatusId === 3 && !orden.invoice
        );

        console.log(`✅ ${ordenesCompletadas.length} órdenes completadas sin factura`);
        return ordenesCompletadas;

    } catch (error) {
        console.error("❌ Error obteniendo órdenes completadas:", error);
        return [];
    }
}

async function obtenerTodasLasFacturas() {
    try {
        console.log("🔄 Obteniendo todas las facturas...");
        
        const response = await fetch(`${API_URL}/invoices/all`);
        if (!response.ok) throw new Error("Error al obtener facturas");

        const facturas = await response.json();
        console.log(`✅ Obtenidas ${facturas.length} facturas`);
        return await enriquecerFacturasConDatosCompletos(facturas);

    } catch (error) {
        console.error("❌ Error obteniendo facturas:", error);
        return [];
    }
}

async function enriquecerOrdenesConDatosCompletos(ordenes) {
    try {
        console.log("🔄 Enriqueciendo datos de órdenes...");
        const [vehiculos, clientes, repuestos] = await Promise.all([
            obtenerVehiculos(),
            obtenerClientes(),
            obtenerRepuestos()
        ]);

        console.log(`🔍 Datos obtenidos: ${vehiculos.length} vehículos, ${clientes.length} clientes, ${repuestos.length} repuestos`);

        const ordenesConDetalles = await Promise.all(
            ordenes.map(async (orden) => {
                try {
                    const detallesRepuestos = await obtenerDetallesOrden(orden.id);
                    
                    const totalRepuestos = detallesRepuestos.reduce((total, detalle) => {
                        const repuesto = repuestos.find(r => r.id === detalle.sparePartId);
                        const subtotal = detalle.quantity * detalle.unitCost;
                        return total + subtotal;
                    }, 0);

                    const vehiculo = vehiculos.find(v => v.id === orden.vehicleId);
                    const cliente = vehiculo ? clientes.find(c => c.id === vehiculo.customerId) : null;

                    const estados = {
                        1: "Pendiente",
                        2: "En Proceso", 
                        3: "Completado"
                    };

                    const tiposServicio = {
                        1: "Mantenimiento preventivo",
                        2: "Reparación",
                        3: "Diagnóstico"
                    };

                    const ordenEnriquecida = {
                        id: orden.id,
                        vehicleId: orden.vehicleId,
                        vehicle: vehiculo ? {
                            brand: vehiculo.brand,
                            model: vehiculo.model,
                            year: vehiculo.year,
                            vin: vehiculo.vin,
                            mileage: vehiculo.mileage
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
                        totalCost: totalRepuestos,
                        notes: orden.notes || "",
                        orderDetails: detallesRepuestos,
                        invoice: orden.invoice || null,
                        _original: orden
                    };

                    console.log(`✅ Orden ${orden.id} enriquecida. Repuestos: ${detallesRepuestos.length}, Total: $${totalRepuestos}`);
                    return ordenEnriquecida;

                } catch (error) {
                    console.error(`❌ Error enriqueciendo orden ${orden.id}:`, error);
                    return {
                        ...orden,
                        orderDetails: [],
                        totalCost: 0,
                        vehicle: { brand: "Error", model: "" },
                        client: { name: "Error" }
                    };
                }
            })
        );

        return ordenesConDetalles;

    } catch (error) {
        console.error("❌ Error enriqueciendo órdenes:", error);
        return ordenes.map(orden => ({
            ...orden,
            orderDetails: [],
            totalCost: 0
        }));
    }
}

async function enriquecerFacturasConDatosCompletos(facturas) {
    try {
        console.log("🔄 Enriqueciendo datos de facturas...");
        const [ordenes, vehiculos, clientes] = await Promise.all([
            obtenerOrdenes(),
            obtenerVehiculos(),
            obtenerClientes()
        ]);

        return facturas.map(factura => {
            const orden = ordenes.find(o => o.id === factura.serviceOrderId);
            const vehiculo = orden ? vehiculos.find(v => v.id === orden.vehicleId) : null;
            const cliente = vehiculo ? clientes.find(c => c.id === vehiculo.customerId) : null;

            const metodosPago = {
                1: "Efectivo",
                2: "Tarjeta", 
                3: "Transferencia"
            };

            const fechaFactura = new Date(factura.issueDate);
            const hoy = new Date();
            const diasDiferencia = Math.floor((hoy - fechaFactura) / (1000 * 60 * 60 * 24));
            const estaPagada = diasDiferencia <= 7;

            return {
                id: factura.id,
                serviceOrderId: factura.serviceOrderId,
                issueDate: factura.issueDate,
                laborCost: factura.laborCost || 0,
                partsTotal: factura.partsTotal || 0,
                total: (factura.laborCost || 0) + (factura.partsTotal || 0),
                paymentMethod: metodosPago[factura.paymentMethod] || `Método ${factura.paymentMethod}`,
                paymentMethodId: factura.paymentMethod,
                estaPagada: estaPagada,
                orden: orden ? {
                    id: orden.id,
                    serviceType: orden.serviceType,
                    entryDate: orden.entryDate,
                    estimatedDeliveryDate: orden.estimatedDeliveryDate
                } : null,
                vehiculo: vehiculo ? {
                    brand: vehiculo.brand,
                    model: vehiculo.model,
                    year: vehiculo.year,
                    vin: vehiculo.vin
                } : { brand: "No encontrado", model: "" },
                cliente: cliente ? {
                    name: cliente.name,
                    phone: cliente.phone,
                    email: cliente.email
                } : { name: "No encontrado" },
                _original: factura
            };
        });

    } catch (error) {
        console.error("❌ Error enriqueciendo facturas:", error);
        return facturas;
    }
}



async function obtenerVehiculos() {
    try {
        const response = await fetch(`${API_URL}/vehicles/all`);
        if (!response.ok) throw new Error("Error al obtener vehículos");
        return await response.json();
    } catch (error) {
        console.error("Error obteniendo vehículos:", error);
        return [];
    }
}

async function obtenerClientes() {
    try {
        const response = await fetch(`${API_URL}/customers/all`);
        if (!response.ok) throw new Error("Error al obtener clientes");
        return await response.json();
    } catch (error) {
        console.error("Error obteniendo clientes:", error);
        return [];
    }
}

async function obtenerRepuestos() {
    try {
        const response = await fetch(`${API_URL}/spareparts/all`);
        if (!response.ok) throw new Error("Error al obtener repuestos");
        const repuestos = await response.json();
        
        return repuestos.map(repuesto => ({
            id: repuesto.id,
            code: repuesto.code || "SIN-CODIGO",
            description: repuesto.description || "Repuesto sin descripción",
            price: parseFloat(repuesto.unitPrice) || 0,
            stock: repuesto.stockQuantity || 0
        }));

    } catch (error) {
        console.error("Error obteniendo repuestos:", error);
        return [];
    }
}

async function obtenerOrdenes() {
    try {
        const response = await fetch(`${API_URL}/serviceorders/all`);
        if (!response.ok) throw new Error("Error al obtener órdenes");
        return await response.json();
    } catch (error) {
        console.error("Error obteniendo órdenes:", error);
        return [];
    }
}

async function obtenerDetallesOrden(ordenId) {
    try {
        console.log(`🔄 Obteniendo detalles para orden: ${ordenId}`);
        
        const response = await fetch(`${API_URL}/orderdetails/by-serviceorder/${ordenId}`);
        
        if (!response.ok) {
            if (response.status === 404) {
                console.warn(`⚠️ Endpoint principal no encontrado, intentando alternativo...`);
                return await obtenerDetallesOrdenAlternativo(ordenId);
            }
            console.warn(`⚠️ No se pudieron obtener detalles para orden ${ordenId}: ${response.status}`);
            return [];
        }
        
        const detalles = await response.json();
        console.log(`✅ Obtenidos ${detalles.length} detalles para orden ${ordenId}`);
        return detalles;
    } catch (error) {
        console.error(`❌ Error obteniendo detalles para orden ${ordenId}:`, error);
        return [];
    }
}

async function obtenerDetallesOrdenAlternativo(ordenId) {
    try {
        console.log(`🔄 Intentando método alternativo para orden: ${ordenId}`);
        
        const response = await fetch(`${API_URL}/orderdetails/all`);
        if (!response.ok) {
            console.warn(`⚠️ No se pudieron obtener todos los detalles`);
            return [];
        }
        
        const todosLosDetalles = await response.json();
        const detallesFiltrados = todosLosDetalles.filter(detalle => detalle.serviceOrderId === ordenId);
        
        console.log(`✅ Obtenidos ${detallesFiltrados.length} detalles (método alternativo) para orden ${ordenId}`);
        return detallesFiltrados;
        
    } catch (error) {
        console.error(`❌ Error en método alternativo:`, error);
        return [];
    }
}



function actualizarMetricasOrdenes(ordenes) {
    const totalPendientes = ordenes.filter(o => o.orderStatusId === 1).length;
    const totalProceso = ordenes.filter(o => o.orderStatusId === 2).length;
    const totalCompletadas = ordenes.filter(o => o.orderStatusId === 3).length;

    document.getElementById("totalPendientes").textContent = totalPendientes;
    document.getElementById("totalProceso").textContent = totalProceso;
    document.getElementById("totalCompletadas").textContent = totalCompletadas;
}

function renderizarTablaOrdenes(ordenes) {
    if (!tablaOrdenes) return;
    
    if (!ordenes || ordenes.length === 0) {
        tablaOrdenes.innerHTML = `
            <tr>
                <td colspan="8" class="no-data">
                    <div style="text-align: center; padding: 20px;">
                        <i style="font-size: 48px; color: #ccc; margin-bottom: 10px;">🔧</i>
                        <p>No tienes órdenes asignadas</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    tablaOrdenes.innerHTML = ordenes.map(orden => {
        const vehicleBrand = orden.vehicle?.brand || 'N/A';
        const vehicleModel = orden.vehicle?.model || '';
        const clientName = orden.client?.name || 'N/A';
        const serviceType = orden.serviceType || 'N/A';
        const orderStatus = orden.orderStatus || 'Pendiente';
        
        const entryDate = orden.entryDate ? new Date(orden.entryDate).toLocaleDateString() : 'N/A';
        const estimatedDate = orden.estimatedDeliveryDate ? new Date(orden.estimatedDeliveryDate).toLocaleDateString() : 'N/A';

        let botones = '';
        if (orden.orderStatusId === 1 || orden.orderStatusId === 2) {
            botones = `
                <button class="btn btn-small btn-blue" onclick="abrirModalActualizar('${orden.id}', ${orden.orderStatusId})">
                    Actualizar Estado
                </button>
            `;
        } else if (orden.orderStatusId === 3 && !orden.invoice) {
            botones = `
                <button class="btn btn-small btn-green" onclick="generarFactura('${orden.id}')">
                    Generar Factura
                </button>
            `;
        } else if (orden.invoice) {
            botones = `<span class="badge estado-completado">Facturada</span>`;
        }

        return `
            <tr>
                <td><strong title="${orden.id}">#${orden.id.substring(0, 8)}</strong></td>
                <td>
                    <div>
                        <strong>${vehicleBrand} ${vehicleModel}</strong>
                        ${orden.vehicle?.year ? `<br><small>Año: ${orden.vehicle.year}</small>` : ''}
                    </div>
                </td>
                <td>
                    <div>
                        <strong>${clientName}</strong>
                        ${orden.client?.phone ? `<br><small>📞 ${orden.client.phone}</small>` : ''}
                    </div>
                </td>
                <td>${serviceType}</td>
                <td>${entryDate}</td>
                <td>${estimatedDate}</td>
                <td>
                    <span class="badge estado-${(orderStatus || 'pendiente').toLowerCase().replace(' ', '-')}">
                        ${orderStatus}
                    </span>
                </td>
                <td style="text-align: center;">
                    ${botones}
                </td>
            </tr>
        `;
    }).join('');

    console.log("✅ Tabla de órdenes del mecánico renderizada con", ordenes.length, "órdenes");
}

function renderizarTablaFacturas(ordenes) {
    if (!tablaFacturas) return;
    
    if (!ordenes || ordenes.length === 0) {
        tablaFacturas.innerHTML = `
            <tr>
                <td colspan="7" class="no-data">
                    <div style="text-align: center; padding: 20px;">
                        <i style="font-size: 48px; color: #ccc; margin-bottom: 10px;">🧾</i>
                        <p>No hay órdenes completadas para facturar</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    tablaFacturas.innerHTML = ordenes.map(orden => {
        const vehicleBrand = orden.vehicle?.brand || 'N/A';
        const vehicleModel = orden.vehicle?.model || '';
        const clientName = orden.client?.name || 'N/A';
        const serviceType = orden.serviceType || 'N/A';

        return `
            <tr>
                <td><strong title="${orden.id}">#${orden.id.substring(0, 8)}</strong></td>
                <td>
                    <div>
                        <strong>${vehicleBrand} ${vehicleModel}</strong>
                        ${orden.vehicle?.year ? `<br><small>Año: ${orden.vehicle.year}</small>` : ''}
                    </div>
                </td>
                <td>
                    <div>
                        <strong>${clientName}</strong>
                        ${orden.client?.phone ? `<br><small>📞 ${orden.client.phone}</small>` : ''}
                    </div>
                </td>
                <td>${serviceType}</td>
                <td style="text-align: right; font-weight: bold;">
                    $${orden.totalCost ? orden.totalCost.toFixed(2) : '0.00'}
                </td>
                <td>
                    <span class="badge estado-completado">Completado</span>
                </td>
                <td style="text-align: center;">
                    <button class="btn btn-small btn-green" onclick="generarFactura('${orden.id}')">
                        Generar Factura
                    </button>
                </td>
            </tr>
        `;
    }).join('');

    console.log("✅ Tabla de facturas renderizada con", ordenes.length, "órdenes");
}

function renderizarTablaTodasFacturas(facturas) {
    if (!tablaTodasFacturas) return;
    
    if (!facturas || facturas.length === 0) {
        tablaTodasFacturas.innerHTML = `
            <tr>
                <td colspan="8" class="no-data">
                    <div style="text-align: center; padding: 20px;">
                        <i style="font-size: 48px; color: #ccc; margin-bottom: 10px;">🧾</i>
                        <p>No se encontraron facturas</p>
                    </div>
                </td>
            </tr>
        `;
        return;
    }

    tablaTodasFacturas.innerHTML = facturas.map(factura => {
        const fecha = factura.issueDate ? new Date(factura.issueDate).toLocaleDateString() : 'N/A';
        const estado = factura.estaPagada ? "Pagada" : "Pendiente";
        const estadoClase = factura.estaPagada ? "pagada" : "pendiente-pago";

        return `
            <tr>
                <td><strong title="${factura.id}">#${factura.id.substring(0, 8)}</strong></td>
                <td>
                    <div>
                        <strong>Orden #${factura.serviceOrderId ? factura.serviceOrderId.substring(0, 8) : 'N/A'}</strong>
                        ${factura.orden?.serviceType ? `<br><small>${factura.orden.serviceType}</small>` : ''}
                    </div>
                </td>
                <td>
                    <div>
                        <strong>${factura.cliente.name}</strong>
                        ${factura.cliente.phone ? `<br><small>📞 ${factura.cliente.phone}</small>` : ''}
                    </div>
                </td>
                <td>
                    <div>
                        <strong>${factura.vehiculo.brand} ${factura.vehiculo.model}</strong>
                        ${factura.vehiculo.year ? `<br><small>Año: ${factura.vehiculo.year}</small>` : ''}
                    </div>
                </td>
                <td style="text-align: right; font-weight: bold;">
                    $${factura.total ? factura.total.toFixed(2) : '0.00'}
                </td>
                <td>${fecha}</td>
                <td>
                    <span class="badge estado-${estadoClase}">
                        ${estado}
                    </span>
                </td>
                <td style="text-align: center;">
                    <button class="btn btn-small btn-blue" onclick="verDetalleFactura('${factura.id}')">
                        Ver Detalle
                    </button>
                </td>
            </tr>
        `;
    }).join('');

    console.log("✅ Tabla de todas las facturas renderizada con", facturas.length, "facturas");
}



function abrirModalActualizar(ordenId, estadoActual) {
    document.getElementById("ordenId").value = ordenId;
    const select = document.getElementById("estadoOrden");
    select.value = estadoActual;
    document.getElementById("modalActualizarOrden").style.display = "block";
}

async function actualizarEstadoOrden() {
    const ordenId = document.getElementById("ordenId").value;
    const nuevoEstado = parseInt(document.getElementById("estadoOrden").value);

    if (!ordenId) {
        mostrarError("No se ha seleccionado una orden");
        return;
    }

    try {
        console.log(`🔄 Actualizando orden ${ordenId} a estado ${nuevoEstado}`);
        
        const updateData = {
            orderStatus: nuevoEstado
        };

        const response = await fetch(`${API_URL}/serviceorders/${ordenId}/status`, {
            method: 'PATCH',
            headers: { 
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(updateData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Error ${response.status}: ${errorText}`);
        }

        console.log("✅ Estado de orden actualizado correctamente");
        mostrarExito("Estado de la orden actualizado correctamente");
        
        document.getElementById("modalActualizarOrden").style.display = "none";
        cargarOrdenesMecanico();
        cargarOrdenesParaFactura();

    } catch (error) {
        console.error("❌ Error actualizando orden:", error);
        mostrarError(`Error al actualizar la orden: ${error.message}`);
    }
}



async function generarFactura(ordenId) {
    try {
        console.log(`🧾 Preparando factura para orden: ${ordenId}`);
        
        const orden = await obtenerOrdenPorId(ordenId);
        if (!orden) {
            mostrarError("No se pudo cargar la información de la orden");
            return;
        }

        document.getElementById("facturaOrdenId").value = ordenId;
        cargarDetallesFactura(orden);
        document.getElementById("modalGenerarFactura").style.display = "block";

    } catch (error) {
        console.error("❌ Error preparando factura:", error);
        mostrarError("Error al preparar la factura");
    }
}

async function obtenerOrdenPorId(ordenId) {
    try {
        console.log(`🔄 Obteniendo orden completa: ${ordenId}`);
        
        const response = await fetch(`${API_URL}/serviceorders/${ordenId}`);
        if (!response.ok) throw new Error("Error al obtener la orden");
        
        const orden = await response.json();
        const detallesRepuestos = await obtenerDetallesOrden(ordenId);
        orden.orderDetails = detallesRepuestos;
        
        console.log(`✅ Orden ${ordenId} obtenida con ${detallesRepuestos.length} repuestos`);
        return orden;
        
    } catch (error) {
        console.error("Error obteniendo orden:", error);
        return null;
    }
}

async function obtenerFacturaPorId(facturaId) {
    try {
        const response = await fetch(`${API_URL}/invoices/${facturaId}`);
        if (!response.ok) throw new Error("Error al obtener la factura");
        return await response.json();
    } catch (error) {
        console.error("Error obteniendo factura:", error);
        return null;
    }
}

function cargarDetallesFactura(orden) {
    const detallesDiv = document.getElementById("facturaDetalles");
    
    console.log("🔍 Detalles de orden para factura:", orden);
    console.log("🔍 OrderDetails:", orden.orderDetails);
    
    if (!orden.orderDetails || orden.orderDetails.length === 0) {
        detallesDiv.innerHTML = `
            <div class="no-data">
                <p>⚠️ No hay repuestos registrados en esta orden</p>
                <p style="font-size: 0.9em; color: #666;">
                    Posibles causas:
                    <br>• Los repuestos no fueron agregados al crear la orden
                    <br>• Error al cargar los datos de repuestos
                    <br>• La orden no tiene detalles asociados
                </p>
            </div>
        `;
        document.getElementById("totalFactura").textContent = "0.00";
        return;
    }

    const totalRepuestos = orden.orderDetails.reduce((total, detalle) => {
        return total + (detalle.quantity * detalle.unitCost);
    }, 0);

    const manoObra = Math.max(totalRepuestos * 0.3, 50);
    const totalFactura = totalRepuestos + manoObra;

    const detallesHtml = `
        <div class="detalle-item">
            <span><strong>Repuestos:</strong></span>
            <span></span>
            <span>$${totalRepuestos.toFixed(2)}</span>
        </div>
        ${orden.orderDetails.map(detalle => `
            <div class="detalle-item" style="margin-left: 20px; font-size: 0.9em; color: #b8b8b8ff;">
                <span>• Repuesto ID: ${detalle.sparePartId}</span>
                <span>${detalle.quantity} x $${detalle.unitCost.toFixed(2)}</span>
                <span>$${(detalle.quantity * detalle.unitCost).toFixed(2)}</span>
            </div>
        `).join('')}
        <div class="detalle-item">
            <span><strong>Mano de obra:</strong></span>
            <span></span>
            <span>$${manoObra.toFixed(2)}</span>
        </div>
        <div class="detalle-item" style="border-top: 1px solid #ccc; padding-top: 10px; font-weight: bold;">
            <span><strong>TOTAL:</strong></span>
            <span></span>
            <span>$${totalFactura.toFixed(2)}</span>
        </div>
    `;

    detallesDiv.innerHTML = detallesHtml;
    document.getElementById("totalFactura").textContent = totalFactura.toFixed(2);
    
    console.log("✅ Detalles de factura cargados. Total:", totalFactura);
}

async function crearFactura() {
    const ordenId = document.getElementById("facturaOrdenId").value;
    const totalElement = document.getElementById("totalFactura");
    const total = parseFloat(totalElement.textContent) || 0;
    const metodoPago = parseInt(document.getElementById("metodoPago").value);

    console.log("🧾 Intentando crear factura para orden:", ordenId, "Total:", total);

    if (!ordenId) {
        mostrarError("No se ha seleccionado una orden");
        return;
    }

    if (total <= 0) {
        const orden = await obtenerOrdenPorId(ordenId);
        if (!orden.orderDetails || orden.orderDetails.length === 0) {
            mostrarError("No se pueden generar facturas sin repuestos. La orden no tiene repuestos asignados.");
        } else {
            mostrarError(`Error: La orden tiene ${orden.orderDetails.length} repuestos pero el total es $0. Verifique los precios.`);
        }
        return;
    }

    try {
        console.log(`🧾 Creando factura para orden: ${ordenId}`);
        
        const orden = await obtenerOrdenPorId(ordenId);
        if (!orden.orderDetails || orden.orderDetails.length === 0) {
            mostrarError("No se pueden generar facturas sin repuestos. Verifique que la orden tenga repuestos asignados.");
            return;
        }

        const partsTotal = total * 0.7;
        const laborCost = total * 0.3;

        const facturaData = {
            serviceOrderId: ordenId,
            laborCost: laborCost,
            partsTotal: partsTotal,
            paymentMethod: metodoPago
        };

        console.log("📦 Datos de factura:", facturaData);

        const response = await fetch(`${API_URL}/invoices`, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(facturaData)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Error ${response.status}: ${errorText}`);
        }

        const nuevaFactura = await response.json();
        console.log("✅ Factura creada:", nuevaFactura);

        mostrarExito(`Factura #${nuevaFactura.id.substring(0, 8)} generada correctamente por $${total.toFixed(2)}`);
        document.getElementById("modalGenerarFactura").style.display = "none";
        
        cargarOrdenesMecanico();
        cargarOrdenesParaFactura();

    } catch (error) {
        console.error("❌ Error creando factura:", error);
        mostrarError(`Error al generar la factura: ${error.message}`);
    }
}

async function verDetalleFactura(facturaId) {
    try {
        console.log(`🔍 Obteniendo detalle de factura: ${facturaId}`);
        
        const factura = await obtenerFacturaPorId(facturaId);
        if (!factura) {
            mostrarError("No se pudo cargar la información de la factura");
            return;
        }

        await cargarDetallesFacturaCompleta(factura);
        document.getElementById("modalDetalleFactura").style.display = "block";

    } catch (error) {
        console.error("❌ Error cargando detalle de factura:", error);
        mostrarError("Error al cargar el detalle de la factura");
    }
}

async function cargarDetallesFacturaCompleta(factura) {
    try {
        console.log("🔄 Cargando detalles completos de factura...");

        const [orden, detallesRepuestos, repuestos] = await Promise.all([
            obtenerOrdenPorId(factura.serviceOrderId),
            obtenerDetallesOrden(factura.serviceOrderId),
            obtenerRepuestos()
        ]);

       
        document.getElementById("detalleFacturaId").textContent = `Factura #${factura.id.substring(0, 8)}`;
        document.getElementById("detalleFacturaFecha").textContent = `Fecha: ${new Date(factura.issueDate).toLocaleDateString()}`;
        document.getElementById("detalleFacturaEstado").textContent = `Estado: ${factura.paymentMethod === 1 ? 'Pagada' : 'Pendiente'}`;
        document.getElementById("detalleFacturaTotal").textContent = `Total: $${((factura.laborCost || 0) + (factura.partsTotal || 0)).toFixed(2)}`;
        document.getElementById("detalleFacturaMetodoPago").textContent = `Método: ${factura.paymentMethod === 1 ? 'Efectivo' : factura.paymentMethod === 2 ? 'Tarjeta' : 'Transferencia'}`;

        
        if (orden) {
            const tiposServicio = {
                1: "Mantenimiento preventivo",
                2: "Reparación",
                3: "Diagnóstico"
            };

            const estados = {
                1: "Pendiente",
                2: "En Proceso", 
                3: "Completado"
            };

            document.getElementById("detalleOrdenInfo").innerHTML = `
                <p><strong>Servicio:</strong> ${tiposServicio[orden.serviceType] || `Servicio #${orden.serviceType}`}</p>
                <p><strong>Estado:</strong> ${estados[orden.orderStatus] || `Estado #${orden.orderStatus}`}</p>
                <p><strong>Fecha de entrada:</strong> ${new Date(orden.entryDate).toLocaleDateString()}</p>
                <p><strong>Fecha estimada de entrega:</strong> ${new Date(orden.estimatedDeliveryDate).toLocaleDateString()}</p>
                ${orden.notes ? `<p><strong>Notas:</strong> ${orden.notes}</p>` : ''}
            `;
        }

        
        if (detallesRepuestos && detallesRepuestos.length > 0) {
            let tablaRepuestos = `
                <table class="tabla-repuestos">
                    <thead>
                        <tr>
                            <th>Repuesto</th>
                            <th>Cantidad</th>
                            <th>Precio Unitario</th>
                            <th>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>
            `;

            let totalRepuestos = 0;

            detallesRepuestos.forEach(detalle => {
                const repuesto = repuestos.find(r => r.id === detalle.sparePartId);
                const subtotal = detalle.quantity * detalle.unitCost;
                totalRepuestos += subtotal;

                tablaRepuestos += `
                    <tr>
                        <td>${repuesto ? repuesto.description : `Repuesto ${detalle.sparePartId}`}</td>
                        <td>${detalle.quantity}</td>
                        <td>$${detalle.unitCost.toFixed(2)}</td>
                        <td>$${subtotal.toFixed(2)}</td>
                    </tr>
                `;
            });

            tablaRepuestos += `
                    </tbody>
                </table>
            `;

            document.getElementById("detalleRepuestosInfo").innerHTML = tablaRepuestos;
        } else {
            document.getElementById("detalleRepuestosInfo").innerHTML = `<p>No hay repuestos registrados en esta orden</p>`;
        }

        
        document.getElementById("detalleSubtotalRepuestos").textContent = `$${(factura.partsTotal || 0).toFixed(2)}`;
        document.getElementById("detalleManoObra").textContent = `$${(factura.laborCost || 0).toFixed(2)}`;
        document.getElementById("detalleTotalFinal").textContent = `$${((factura.laborCost || 0) + (factura.partsTotal || 0)).toFixed(2)}`;

    } catch (error) {
        console.error("❌ Error cargando detalles completos:", error);
        mostrarError("Error al cargar los detalles completos de la factura");
    }
}



function aplicarFiltrosFacturas() {
    const estado = document.getElementById("filtroEstadoFactura").value;
    const mes = document.getElementById("filtroMesFactura").value;
    
    console.log(`Aplicando filtros: Estado=${estado}, Mes=${mes}`);
    cargarTodasLasFacturas();
}

function limpiarFiltrosFacturas() {
    document.getElementById("filtroEstadoFactura").value = "todas";
    document.getElementById("filtroMesFactura").value = "";
    cargarTodasLasFacturas();
}



function actualizarFechaActual() {
    const ahora = new Date();
    const opciones = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    const fechaElement = document.getElementById("fechaActual");
    if (fechaElement) {
        fechaElement.textContent = ahora.toLocaleDateString('es-ES', opciones);
    }
}

function mostrarError(mensaje) {
    alert(`❌ Error: ${mensaje}`);
}

function mostrarExito(mensaje) {
    alert(`✅ ${mensaje}`);
}


window.abrirModalActualizar = abrirModalActualizar;
window.generarFactura = generarFactura;
window.verDetalleFactura = verDetalleFactura;
window.aplicarFiltrosFacturas = aplicarFiltrosFacturas;
window.limpiarFiltrosFacturas = limpiarFiltrosFacturas;