const token = localStorage.getItem("token");
if (!token) {
    window.location.href = "login.html";
}

let chart = null;

document.getElementById("userName").textContent = localStorage.getItem("name");

// mes actual por defecto en el selector (formato yyyy-MM)
const today = new Date();
const currentMonth = today.getFullYear() + "-" + String(today.getMonth() + 1).padStart(2, "0");
document.getElementById("monthInput").value = currentMonth;
document.getElementById("date").value = currentMonth + "-" + String(today.getDate()).padStart(2, "0");

function getHeaders() {
    return {
        "Content-Type": "application/json",
        "Authorization": "Bearer " + token
    };
}

function getYearAndMonth() {
    const value = document.getElementById("monthInput").value; // "2026-10"
    const parts = value.split("-");
    return { year: parts[0], month: parts[1] };
}

function formatMoney(amount) {
    return amount.toFixed(2) + " €";
}

async function apiGet(url) {
    const response = await fetch(url, { headers: getHeaders() });

    // si el token ha caducado volvemos al login
    if (response.status === 401) {
        logout();
        return null;
    }

    return await response.json();
}

async function loadCategories() {
    const categories = await apiGet("/api/categories");
    const select = document.getElementById("category");
    select.innerHTML = "";

    categories.forEach(c => {
        const option = document.createElement("option");
        option.value = c.id;
        option.textContent = c.name + (c.type === "Income" ? " (ingreso)" : "");
        select.appendChild(option);
    });
}

async function loadSummary() {
    const { year, month } = getYearAndMonth();
    const summary = await apiGet(`/api/transactions/summary?year=${year}&month=${month}`);

    document.getElementById("totalIncome").textContent = formatMoney(summary.totalIncome);
    document.getElementById("totalExpenses").textContent = formatMoney(summary.totalExpenses);

    const balance = document.getElementById("balance");
    balance.textContent = formatMoney(summary.balance);
    balance.className = summary.balance >= 0 ? "income" : "expense";

    drawChart(summary.expensesByCategory);
}

function drawChart(data) {
    if (chart) {
        chart.destroy();
    }

    document.getElementById("noData").style.display = data.length === 0 ? "block" : "none";

    chart = new Chart(document.getElementById("chart"), {
        type: "doughnut",
        data: {
            labels: data.map(x => x.category),
            datasets: [{
                data: data.map(x => x.total)
            }]
        }
    });
}

async function loadTransactions() {
    const { year, month } = getYearAndMonth();
    const transactions = await apiGet(`/api/transactions?year=${year}&month=${month}`);
    const tbody = document.getElementById("transactionsTable");
    tbody.innerHTML = "";

    if (transactions.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No hay movimientos</td></tr>';
        return;
    }

    transactions.forEach(t => {
        const tr = document.createElement("tr");
        const sign = t.type === "Income" ? "+" : "-";
        const cssClass = t.type === "Income" ? "income" : "expense";

        tr.innerHTML = `
            <td>${new Date(t.date).toLocaleDateString("es-ES")}</td>
            <td></td>
            <td></td>
            <td class="text-end ${cssClass}">${sign}${formatMoney(t.amount)}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-danger" onclick="deleteTransaction(${t.id})">Borrar</button>
            </td>`;

        // estos los pongo con textContent para evitar meter html
        tr.children[1].textContent = t.description;
        tr.children[2].textContent = t.categoryName;
        tbody.appendChild(tr);
    });
}

async function deleteTransaction(id) {
    if (!confirm("¿Borrar este movimiento?")) {
        return;
    }

    await fetch("/api/transactions/" + id, {
        method: "DELETE",
        headers: getHeaders()
    });

    reload();
}

document.getElementById("transactionForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const data = {
        description: document.getElementById("description").value,
        amount: parseFloat(document.getElementById("amount").value),
        date: document.getElementById("date").value,
        categoryId: parseInt(document.getElementById("category").value)
    };

    const response = await fetch("/api/transactions", {
        method: "POST",
        headers: getHeaders(),
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        alert("Error al guardar el movimiento");
        return;
    }

    document.getElementById("description").value = "";
    document.getElementById("amount").value = "";
    reload();
});

document.getElementById("categoryForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const data = {
        name: document.getElementById("categoryName").value,
        type: document.getElementById("categoryType").value
    };

    const response = await fetch("/api/categories", {
        method: "POST",
        headers: getHeaders(),
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        alert("Error al crear la categoría");
        return;
    }

    document.getElementById("categoryName").value = "";
    loadCategories();
});

function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("name");
    window.location.href = "login.html";
}

function reload() {
    loadSummary();
    loadTransactions();
}

document.getElementById("logoutBtn").addEventListener("click", logout);
document.getElementById("monthInput").addEventListener("change", reload);

loadCategories();
reload();
