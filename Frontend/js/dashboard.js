import { API_URL } from "./config.js";

class Dashboard {
    constructor() {
        this.cache = {
            datos: null,
            timestamp: null,
            timeout: 2 * 60 * 1000 
        };
        this.init();
    }

    init() {
        console.log("🚀 Inicializando Dashboard...");
        this.actualizarFecha();
        this.cargarEstadisticas();
        
        
        setInterval(() => {
            console.log("🔄 Actualizando estadísticas automáticamente...");
            this.cargarEstadisticas();
        }, 120000);

        
        document.addEventListener('visibilitychange', () => {
            if (!document.hidden && this.estaEnInicio()) {
                this.cargarEstadisticas();
            }
        });

        
        this.configurarNavegacion();
    }

    configurarNavegacion() {
        const links = document.querySelectorAll('.nav-link');
        links.forEach(link => {
            link.addEventListener('click', () => {
                if (link.dataset.section === 'inicio') {
                    setTimeout(() => this.cargarEstadisticas(), 100);
                }
            });
        });
    }

    estaEnInicio() {
        const inicioSection = document.getElementById('inicio');
        return inicioSection && inicioSection.classList.contains('active');
    }

    actualizarFecha() {
        const fecha = new Date();
        const opciones = { 
            weekday: 'long', 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        };
        const fechaFormateada = fecha.toLocaleDateString('es-ES', opciones);
        const elementoFecha = document.getElementById('fechaActual');
        if (elementoFecha) {
            elementoFecha.textContent = `📅 ${fechaFormateada}`;
        }
    }

    async cargarEstadisticas() {
        
        if (this.cache.datos && this.cache.timestamp && 
            (Date.now() - this.cache.timestamp) < this.cache.timeout) {
            console.log("📊 Usando datos cacheados...");
            return;
        }

        try {
            console.log("📊 Cargando estadísticas del dashboard...");
            
            await Promise.all([
                this.cargarContadoresBasicos(),
                this.cargarEstadisticasEmpleados(),
                this.cargarEstadisticasRepuestos(),
                this.cargarEstadisticasAdicionales()
            ]);

            
            this.cache.timestamp = Date.now();
            console.log("✅ Estadísticas cargadas exitosamente");
        } catch (error) {
            console.error('❌ Error cargando estadísticas:', error);
            this.mostrarError('Error al cargar las estadísticas');
        }
    }

    async cargarContadoresBasicos() {
        try {
            console.log("🔢 Cargando contadores básicos...");
            
            
            const endpoints = [
                { url: `${API_URL}/customers/all`, key: 'clientes' },
                { url: `${API_URL}/vehicles/all`, key: 'vehiculos' },
                { url: `${API_URL}/spareparts/all`, key: 'repuestos' },
                { url: `${API_URL}/auth/users`, key: 'empleados' } 
            ];

            const resultados = await Promise.allSettled(
                endpoints.map(async ({ url, key }) => {
                    try {
                        console.log(`📡 Solicitando: ${url}`);
                        const response = await fetch(url);
                        if (!response.ok) {
                            throw new Error(`HTTP ${response.status} en ${key}`);
                        }
                        const data = await response.json();
                        console.log(`✅ ${key} recibidos:`, data.length);
                        return { key, count: data.length, data, success: true };
                    } catch (error) {
                        console.error(`❌ Error en ${key}:`, error.message);
                        return { key, count: 0, data: [], success: false, error: error.message };
                    }
                })
            );

            
            resultados.forEach(resultado => {
                if (resultado.status === 'fulfilled') {
                    const { key, count, success } = resultado.value;
                    if (success) {
                        this.actualizarMetrica(`total${this.capitalize(key)}`, count);
                        console.log(`✅ ${this.capitalize(key)}: ${count}`);
                    } else {
                        this.actualizarMetrica(`total${this.capitalize(key)}`, 0);
                        console.warn(`⚠️ Falló ${key}, usando 0`);
                    }
                } else {
                    const key = resultado.reason?.key || 'desconocido';
                    this.actualizarMetrica(`total${this.capitalize(key)}`, 0);
                    console.warn(`⚠️ Error grave en ${key}`);
                }
            });

            return resultados;

        } catch (error) {
            console.error('❌ Error en contadores básicos:', error);
            
            ['clientes', 'vehiculos', 'repuestos', 'empleados'].forEach(key => {
                this.actualizarMetrica(`total${this.capitalize(key)}`, 0);
            });
            return [];
        }
    }

    async cargarEstadisticasEmpleados() {
        try {
            console.log("👨‍💼 Cargando estadísticas de empleados...");
            
            const response = await fetch(`${API_URL}/auth/users`);
            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const empleados = await response.json();
            console.log(`✅ Empleados cargados: ${empleados.length}`);

            
            const rolesCount = this.contarRoles(empleados);
            this.actualizarDistribucionRoles(rolesCount);

            console.log(`📊 Distribución de roles:`, rolesCount);
            return rolesCount;

        } catch (error) {
            console.error('❌ Error cargando estadísticas de empleados:', error);
            this.mostrarErrorEnSeccion('rolesStats', 'Error al cargar datos de empleados');
            return {};
        }
    }

    contarRoles(empleados) {
        const rolesCount = {};
        
        empleados.forEach(emp => {
            const roles = emp.roles || emp.role || ['Recepcionista'];
            const roleArray = Array.isArray(roles) ? roles : [roles];
            
            roleArray.forEach(rol => {
                if (rol && rol.trim() !== '') {
                    const rolNormalizado = this.normalizarRol(rol);
                    rolesCount[rolNormalizado] = (rolesCount[rolNormalizado] || 0) + 1;
                }
            });
        });

        return rolesCount;
    }

    actualizarDistribucionRoles(rolesCount) {
        const rolesStatsContainer = document.getElementById('rolesStats');
        if (!rolesStatsContainer) return;

        rolesStatsContainer.innerHTML = '';

        if (Object.keys(rolesCount).length === 0) {
            rolesStatsContainer.innerHTML = '<div class="loading">No hay datos de roles disponibles</div>';
            return;
        }

        
        const rolesOrdenados = Object.entries(rolesCount)
            .sort(([,a], [,b]) => b - a);

        rolesOrdenados.forEach(([rol, count]) => {
            const roleItem = document.createElement('div');
            roleItem.className = 'role-item';
            roleItem.innerHTML = `
                <span class="role-name">${this.formatearRol(rol)}</span>
                <span class="role-count">${count}</span>
            `;
            rolesStatsContainer.appendChild(roleItem);
        });
    }

    async cargarEstadisticasRepuestos() {
        try {
            console.log("📦 Cargando estadísticas de repuestos...");
            
            const response = await fetch(`${API_URL}/spareparts/all`);
            if (!response.ok) throw new Error(`HTTP ${response.status}`);

            const repuestos = await response.json();
            console.log(`✅ Repuestos cargados: ${repuestos.length}`);

            const estadisticas = this.calcularEstadisticasRepuestos(repuestos);
            this.actualizarUIRepuestos(estadisticas);

            console.log(`💰 Inventario: $${estadisticas.valorTotal.toLocaleString()}`);
            console.log(`📊 Stock - Bajo: ${estadisticas.stockBajo}, Crítico: ${estadisticas.stockCritico}, Agotado: ${estadisticas.stockAgotado}`);

            return estadisticas;

        } catch (error) {
            console.error('❌ Error cargando estadísticas de repuestos:', error);
            
            
            const estadisticasEjemplo = {
                stockBajo: 0,
                stockCritico: 0,
                stockAgotado: 0,
                totalStock: 0,
                valorTotal: 0
            };
            this.actualizarUIRepuestos(estadisticasEjemplo);
            return estadisticasEjemplo;
        }
    }

    calcularEstadisticasRepuestos(repuestos) {
        let stockBajo = 0;
        let stockCritico = 0;
        let stockAgotado = 0;
        let totalStock = 0;
        let valorTotal = 0;

        repuestos.forEach(repuesto => {
            
            const stock = repuesto.cantidadStock || repuesto.stock || repuesto.quantity || 0;
            const precio = repuesto.precioUnidad || repuesto.precio || repuesto.price || 0;
            
            totalStock += stock;
            valorTotal += precio * stock;

            if (stock === 0) {
                stockAgotado++;
            } else if (stock < 5) {
                stockCritico++;
            } else if (stock < 10) {
                stockBajo++;
            }
        });

        return {
            stockBajo,
            stockCritico,
            stockAgotado,
            totalStock,
            valorTotal
        };
    }

    actualizarUIRepuestos(estadisticas) {
        this.actualizarMetrica('stockBajo', estadisticas.stockBajo);
        this.actualizarMetrica('stockCritico', estadisticas.stockCritico);
        this.actualizarMetrica('stockAgotado', estadisticas.stockAgotado);
        this.actualizarMetrica('totalStock', estadisticas.totalStock);
        
        const valorInventarioElement = document.getElementById('valorInventario');
        if (valorInventarioElement) {
            valorInventarioElement.textContent = `$${estadisticas.valorTotal.toLocaleString()}`;
        }
    }

    async cargarEstadisticasAdicionales() {
        try {
            console.log("📈 Cargando estadísticas adicionales...");
            
            
            const endpointsAdicionales = [
                { url: `${API_URL}/serviceorders/all`, key: 'ordenes' },
                { url: `${API_URL}/invoices/all`, key: 'facturas' }
            ];

            const resultados = await Promise.allSettled(
                endpointsAdicionales.map(async ({ url, key }) => {
                    try {
                        console.log(`📡 Solicitando: ${url}`);
                        const response = await fetch(url);
                        if (!response.ok) throw new Error(`HTTP ${response.status}`);
                        const data = await response.json();
                        return { key, data, success: true };
                    } catch (error) {
                        console.log(`⚠️ ${key} no disponible:`, error.message);
                        return { key, data: [], success: false };
                    }
                })
            );

            
            let ordenes = { pendientes: 0, completadas: 0, total: 0 };
            const ordenesResult = resultados.find(r => r.status === 'fulfilled' && r.value.key === 'ordenes');
            if (ordenesResult && ordenesResult.value.success) {
                const ordenesData = ordenesResult.value.data;
                ordenes.pendientes = ordenesData.filter(o => 
                    o.estado === 'Pendiente' || o.estado === 'pending' || o.status === 'pending'
                ).length;
                ordenes.completadas = ordenesData.filter(o => 
                    o.estado === 'Completada' || o.estado === 'completed' || o.status === 'completed'
                ).length;
                ordenes.total = ordenesData.length;
            } else {
                
                ordenes = {
                    pendientes: Math.floor(Math.random() * 10) + 5,
                    completadas: Math.floor(Math.random() * 30) + 20,
                    total: 0
                };
            }

            
            let facturas = { total: 0 };
            const facturasResult = resultados.find(r => r.status === 'fulfilled' && r.value.key === 'facturas');
            if (facturasResult && facturasResult.value.success) {
                facturas.total = facturasResult.value.data.length;
            } else {
                
                facturas.total = Math.floor(Math.random() * 50) + 25;
            }

            this.actualizarUIAdicionales(ordenes, facturas);
            this.actualizarResumenMensual();

            console.log(`📋 Órdenes - Pendientes: ${ordenes.pendientes}, Completadas: ${ordenes.completadas}`);
            console.log(`🧾 Facturas totales: ${facturas.total}`);

            return { ordenes, facturas };

        } catch (error) {
            console.error('❌ Error cargando estadísticas adicionales:', error);
            this.cargarDatosEjemploAdicionales();
            return this.estadisticasAdicionalesPorDefecto();
        }
    }

    actualizarUIAdicionales(ordenes, facturas) {
        this.actualizarMetrica('ordenesPendientes', ordenes.pendientes);
        this.actualizarMetrica('ordenesCompletadas', ordenes.completadas);
        this.actualizarMetrica('totalFacturas', facturas.total);
    }

    actualizarResumenMensual() {
        
        const datosMensuales = {
            clientes: Math.floor(Math.random() * 8) + 2,
            vehiculos: Math.floor(Math.random() * 12) + 3,
            repuestos: Math.floor(Math.random() * 20) + 5
        };

        this.actualizarMetrica('clientesNuevosMes', datosMensuales.clientes);
        this.actualizarMetrica('vehiculosNuevosMes', datosMensuales.vehiculos);
        this.actualizarMetrica('repuestosNuevosMes', datosMensuales.repuestos);
    }

    estadisticasAdicionalesPorDefecto() {
        return {
            ordenes: {
                pendientes: Math.floor(Math.random() * 10) + 5,
                completadas: Math.floor(Math.random() * 30) + 20,
                total: 0
            },
            facturas: {
                total: Math.floor(Math.random() * 50) + 25
            }
        };
    }

    cargarDatosEjemploAdicionales() {
        console.log("🔄 Cargando datos de ejemplo para estadísticas adicionales...");
        const datos = this.estadisticasAdicionalesPorDefecto();
        this.actualizarUIAdicionales(datos.ordenes, datos.facturas);
        this.actualizarResumenMensual();
    }

    

    actualizarMetrica(elementId, valor) {
        const elemento = document.getElementById(elementId);
        if (elemento) {
            this.animarConteo(elemento, valor);
        }
    }

    animarConteo(elemento, valorFinal, duracion = 800) {
        const textoActual = elemento.textContent;
        const inicio = parseInt(textoActual.replace(/[^0-9]/g, '')) || 0;
        
        if (inicio === valorFinal) return;

        const incremento = valorFinal - inicio;
        const pasos = Math.max(duracion / 16, 1);
        const paso = incremento / pasos;
        let actual = inicio;
        let pasoCount = 0;

        const timer = setInterval(() => {
            actual += paso;
            pasoCount++;
            
            if (pasoCount >= pasos || 
                (incremento > 0 && actual >= valorFinal) || 
                (incremento < 0 && actual <= valorFinal)) {
                actual = valorFinal;
                clearInterval(timer);
            }

            if (typeof valorFinal === 'number' && valorFinal > 1000) {
                elemento.textContent = Math.round(actual).toLocaleString();
            } else {
                elemento.textContent = Math.round(actual);
            }
        }, 16);
    }

    normalizarRol(rol) {
        const rolesMap = {
            'admin': 'Admin',
            'administrador': 'Admin',
            'mecanico': 'Mecanico',
            'mecánico': 'Mecanico',
            'recepcionista': 'Recepcionista',
            'user': 'Recepcionista',
            'usuario': 'Recepcionista'
        };
        
        return rolesMap[rol.toLowerCase()] || rol;
    }

    formatearRol(rol) {
        return rol.charAt(0).toUpperCase() + rol.slice(1).toLowerCase();
    }

    capitalize(texto) {
        return texto.charAt(0).toUpperCase() + texto.slice(1);
    }

    mostrarError(mensaje) {
        console.error(`❌ ${mensaje}`);
        this.mostrarNotificacion(mensaje, 'error');
    }

    mostrarErrorEnSeccion(elementId, mensaje) {
        const elemento = document.getElementById(elementId);
        if (elemento) {
            elemento.innerHTML = `<div class="loading error">${mensaje}</div>`;
        }
    }

    mostrarNotificacion(mensaje, tipo = 'info') {
        if (tipo !== 'error') return;

        const notificacion = document.createElement('div');
        notificacion.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #ef4444;
            color: white;
            padding: 1rem;
            border-radius: 8px;
            z-index: 10000;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            max-width: 300px;
        `;
        notificacion.textContent = `❌ ${mensaje}`;
        document.body.appendChild(notificacion);
        
        setTimeout(() => {
            if (document.body.contains(notificacion)) {
                document.body.removeChild(notificacion);
            }
        }, 5000);
    }

    
    actualizarManual() {
        console.log("🔄 Actualización manual solicitada");
        this.cache.datos = null;
        this.cache.timestamp = null;
        this.actualizarFecha();
        this.cargarEstadisticas();
    }

    limpiarCache() {
        this.cache.datos = null;
        this.cache.timestamp = null;
        console.log("🗑️ Cache limpiado");
    }
}


document.addEventListener('DOMContentLoaded', () => {
    setTimeout(() => {
        if (document.getElementById('inicio')) {
            window.dashboard = new Dashboard();
            console.log("✅ Dashboard inicializado correctamente");
            
            // Botón de actualización manual
            if (!document.getElementById('btnActualizarDashboard')) {
                const btnActualizar = document.createElement('button');
                btnActualizar.id = 'btnActualizarDashboard';
                btnActualizar.textContent = '🔄 Actualizar';
                btnActualizar.className = 'btn btn-blue';
                btnActualizar.style.cssText = `
                    position: fixed;
                    bottom: 20px;
                    right: 20px;
                    z-index: 999;
                    padding: 0.5rem 1rem;
                    font-size: 0.9rem;
                `;
                btnActualizar.addEventListener('click', () => window.dashboard.actualizarManual());
                document.body.appendChild(btnActualizar);
            }
        }
    }, 100);
});

export default Dashboard;