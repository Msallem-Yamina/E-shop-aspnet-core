// Met à jour l'affichage du panier après une action AJAX
function majPanier(id, data) {
    $("#totalapayer").text(data.total.toFixed(2));
    $("#quantite_" + id).text(data.quantite);
    $("#rquantite_" + id).text(data.quantite);
    $("#total_" + id).text(data.totalRow.toFixed(2));
    majBadgePanier();
}

// Rafraîchit le nombre d'articles affiché dans la navbar
function majBadgePanier() {
    $.get("/Panier/Count", function (data) {
        var badge = $("#cartCount");
        badge.text(data.count);
        badge.toggleClass("d-none", data.count === 0);
    });
}

// Bouton +
$(document).on("click", ".PlusProducts", function (event) {
    event.preventDefault();
    var id = $(this).attr("data-id");
    $.post("/Panier/PlusProduct", { id: id }, function (data) {
        if (data.ct == 1) {
            majPanier(id, data);
        }
    });
});

// Bouton -
$(document).on("click", ".MinProducts", function (event) {
    event.preventDefault();
    var id = $(this).attr("data-id");
    $.post("/Panier/MinusProduct", { id: id }, function (data) {
        if (data.ct == 1) {
            majPanier(id, data);
        } else {
            // La quantité est tombée à 0 : la ligne est supprimée
            window.location.href = "/Panier";
        }
    });
});

// Bouton supprimer
$(document).on("click", ".RemoveLink", function (event) {
    event.preventDefault();
    var id = $(this).attr("data-id");
    $.post("/Panier/RemoveProduct", { id: id }, function () {
        window.location.href = "/Panier";
    });
});

// Clic sur une carte article : ouvre sa fiche
$(document).on("click", ".product-card", function (event) {
    if ($(event.target).closest("a, button").length) return;
    window.location.href = this.dataset.href;
});

$(document).ready(function () {
    majBadgePanier();

    // Met en évidence la page courante dans le sidebar
    $(".app-sidebar .nav-link").filter(function () {
        return this.pathname.toLowerCase() === window.location.pathname.toLowerCase();
    }).addClass("active");
});