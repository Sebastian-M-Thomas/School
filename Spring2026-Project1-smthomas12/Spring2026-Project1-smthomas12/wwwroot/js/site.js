const characters = {
    luffy: "Monkey D. Luffy - Captain of the Straw Hat pirates.",
    zoro: "Roranoa Zoro - Swordsman of the Straw Hat pirates and right hand to Monkey D. Luffy.",
    nami: "Nami - Navigator of the Straw Hat pirates.",
    usopp: "Usopp - Sniper of the Straw Hat pirates.",
    sanji: "Vinsmoke Sanji - Cook of the Straw Hat pirates and left hand to Monkey D. Luffy.",
};

function showCharacter(name) {
    const infoBox = document.getElementById("characterInfo");
    infoBox.style.display = "block";
    infoBox.textContent = characters[name];
}