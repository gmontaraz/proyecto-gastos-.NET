// si ya hay token vamos directos a la app
if (localStorage.getItem("token")) {
    window.location.href = "index.html";
}

const loginForm = document.getElementById("loginForm");
const registerForm = document.getElementById("registerForm");
const errorDiv = document.getElementById("error");

document.getElementById("showRegister").addEventListener("click", function (e) {
    e.preventDefault();
    loginForm.classList.add("d-none");
    registerForm.classList.remove("d-none");
    errorDiv.classList.add("d-none");
});

document.getElementById("showLogin").addEventListener("click", function (e) {
    e.preventDefault();
    registerForm.classList.add("d-none");
    loginForm.classList.remove("d-none");
    errorDiv.classList.add("d-none");
});

loginForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    const data = {
        email: document.getElementById("loginEmail").value,
        password: document.getElementById("loginPassword").value
    };

    await sendAuth("/api/auth/login", data);
});

registerForm.addEventListener("submit", async function (e) {
    e.preventDefault();

    const data = {
        name: document.getElementById("registerName").value,
        email: document.getElementById("registerEmail").value,
        password: document.getElementById("registerPassword").value
    };

    await sendAuth("/api/auth/register", data);
});

async function sendAuth(url, data) {
    const response = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const text = await response.text();
        showError(text);
        return;
    }

    const result = await response.json();
    localStorage.setItem("token", result.token);
    localStorage.setItem("name", result.name);
    window.location.href = "index.html";
}

function showError(text) {
    // los errores de validacion vienen en json, los mios en texto plano
    try {
        const json = JSON.parse(text);
        if (json.errors) {
            text = Object.values(json.errors)[0][0];
        }
    } catch {
        // no era json
    }

    errorDiv.textContent = text || "Ha habido un error";
    errorDiv.classList.remove("d-none");
}
