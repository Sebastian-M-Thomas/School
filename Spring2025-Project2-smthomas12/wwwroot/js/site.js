

const API_KEY = "89e6f9acd0f9cc79989f6b2058d22b248e2da171";

// ── Background Images ──────────────────────────────────────;
const backgrounds = [
    "url('../Images/DeathStar.jpg')",
    "url('../Images/Tatooine.jpg')",
    "url('../Images/Hoth.jpg')",
    "url('../Images/Coruscant.jpg')",
    "url('../Images/Falcon.jpg')",
    "url('../Images/Hyperspace.WEBP')"
];
let bgIndex = 0;

// Feeling Lucky
let firstResultUrl = null;

window.addEventListener("load", () => {
    document.getElementById("searchBtn").addEventListener("click", search);
    document.getElementById("luckyBtn").addEventListener("click", feelingLucky);
    document.getElementById("timeBtn").addEventListener("click", showTime);
    document.getElementById("legendaryBtn").addEventListener("click", feelTheForce);
    document.getElementById("siteTitle").addEventListener("click", cycleBackground);

    
    document.getElementById("query").addEventListener("keydown", e => {
        if (e.key === "Enter") search();
    });
});

// ── Knowledge Graph ────────────────────────────────────────
function displayKnowledgeGraph(kg) {
    if (!kg) return "";
    return `
        <div class="kg-box">
            <h2>Knowledge Graph</h2>
            <h3>${kg.title || ""}</h3>
            <p>${kg.description || ""}</p>
            ${kg.imageUrl ? `<img src="${kg.imageUrl}" class="kg-img" alt="${kg.title}">` : ""}
        </div>
    `;
}

// ── People Also Ask ────────────────────────────────────────
function displayPeopleAlsoAsk(paa) {
    if (!paa || paa.length === 0) return "";
    let html = `<div class="paa-box"><h2>People Also Asked</h2>`;
    paa.forEach(q => html += `<p>▸ <span class="paa-link" data-query="${q.question}">${q.question}</span></p>`);
    html += `</div>`;
    return html;
}

// ── Related Searches ───────────────────────────────────────
function displayRelatedSearches(related) {
    if (!related || related.length === 0) return "";
    let html = `<div class="related-box"><h2>Related Searches</h2>`;
    related.forEach(r => html += `<p>▸ <span class="related-link" data-query="${r.query}">${r.query}</span></p>`);
    html += `</div>`;
    return html;
}

// ── Search ─────────────────────────────────────────────────
function search() {
    const query = document.getElementById("query").value.trim();
    if (!query) return alert("Enter a search term, young Padawan.");

    const resultsEl = document.getElementById("searchResults");
    resultsEl.style.display = "block";
    resultsEl.innerHTML = `
        <div style="text-align:center; padding: 30px; font-family:'Orbitron',sans-serif;
             font-size:12px; letter-spacing:3px; color: var(--saber-blue);">
            SCANNING THE GALAXY...
        </div>`;

    fetch("https://google.serper.dev/search", {
        method: "POST",
        headers: {
            "X-API-KEY": API_KEY,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ q: query })
    })
        .then(res => res.json())
        .then(data => {
            console.log(data);
            let html = "";

            if (data.knowledgeGraph) {
                html += displayKnowledgeGraph(data.knowledgeGraph);
            }

            if (data.organic && data.organic.length > 0) {
                firstResultUrl = data.organic[0].link;
                html += `<div class="organic-box"><h2>Search Results</h2>`;
                data.organic.forEach(item => {
                    html += `
                    <div class="search-item">
                        <h3><a href="${item.link}" target="_blank">${item.title}</a></h3>
                        <p>${item.snippet}</p>
                    </div>`;
                });
                html += `</div>`;
            } else {
                html += `<p style="color:var(--text-dim); text-align:center;">
                No transmissions found across the galaxy.</p>`;
                firstResultUrl = null;
            }

            if (data.peopleAlsoAsk) html += displayPeopleAlsoAsk(data.peopleAlsoAsk);
            if (data.relatedSearches) html += displayRelatedSearches(data.relatedSearches);

            resultsEl.innerHTML = html;

            document.querySelectorAll(".related-link").forEach(el => {
                el.addEventListener("click", () => {
                    document.getElementById("query").value = el.dataset.query;
                    search();
                });
            });

            document.querySelectorAll(".paa-link").forEach(el => {
                el.addEventListener("click", () => {
                    document.getElementById("query").value = el.dataset.query;
                    search();
                });
            });
        })

        .catch(err => {
            console.error("Transmission error:", err);
            resultsEl.innerHTML = `<p style="color:#ff4444; text-align:center;">
            Transmission failed. The Force is disturbed.</p>`;
        });
}

// ── Feeling Lucky ──────────────────────────────────────────
function feelingLucky() {
    if (firstResultUrl) {
        window.open(firstResultUrl, "_blank");
    } else {
        alert("No transmission to open! Search the galaxy first.");
    }
}

// ── Time ──────────────────────────────────────────
function showTime() {
    const now = new Date();
    const hh = String(now.getHours()).padStart(2, "0");
    const mm = String(now.getMinutes()).padStart(2, "0");
    const timeDiv = document.getElementById("time");
    timeDiv.innerText = `GALACTIC STANDARD TIME — ${hh}:${mm}`;
    timeDiv.style.display = "block";

    if (typeof $ !== "undefined" && typeof $.ui !== "undefined") {
        $(timeDiv).dialog({ title: "Galactic Standard Time", modal: true, width: 320 });
    }
}

// ── Background Cycling ─────────────────────────────────────
function cycleBackground() {
    bgIndex = (bgIndex + 1) % backgrounds.length;
    document.body.style.backgroundImage = backgrounds[bgIndex];
}

// ── Feel the Force (fun button) ────────────────────────────
function feelTheForce() {
    window.open("https://www.youtube.com/watch?v=_D0ZQPqeJkk", "_blank"); // Binary Sunset / Force Theme
}