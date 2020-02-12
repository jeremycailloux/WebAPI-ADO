//#region Variables globales et initialisation
//---------------------------------------------
const uri = "api/TodoList"; // uri de l'API web
const MAXDATE = new Date("9999-12-31T23:59:59.999Z");
let taches = []; // liste des tâches

// Renvoie une chaîne au format 'yyyy-mm-dd' en temps local à partir d'un objet Date
Date.prototype.toStandardFormat = function () {
   return new Date(this.valueOf() - this.getTimezoneOffset() * 6e4).toISOString().slice(0, 10);
};

// Initialise l'application
function initAppli() {
   document.getElementById("btnAjouter").onclick = function () {
      initFormulaire(-1);
   };

   getTaches();   // Récupère et affiche la liste des tâches
}
//#endregion 

//#region Fonctions de dialogue avec l'API web
//---------------------------------------------

// Récupère toutes les tâches par XHR
function getTaches() {
   const req = new XMLHttpRequest();
   req.onload = function () {
      if (this.status === 200) {
         // Récupère la réponse au format texte json
         const rep = this.responseText;
         //console.log(this.response);

         // Desérialise le texte json en objet Array
         // en transformant les dates en objets Date
         taches = JSON.parse(rep, (key, value) => {
            let res = value;
            if (key.substr(0, 4) === "date") res = new Date(value);

            return res;
         });

         // Affiche les tâches dans le tableau HTML
         afficherTaches();
      }
      else {
         console.log("erreur requête GET : " + this.status + " " + this.statusText);
      }
   };
   req.open('GET', uri, true);
   req.send(null);
}

// Envoie une nouvelle tâche par XHR
function postTache(tache) {
   // Envoie la tâche à l'API
   const req = new XMLHttpRequest();
   req.onload = function () {
      if (this.status < 300) {
         // Ajoute la tâche au tableau et rafraichit le visuel
         taches.push(tache);
         afficherTaches();
         afficherFormulaire(false);
      }
      else {
         console.log("erreur requête POST : " + this.status + " " + this.statusText);
      }
   };

   req.open('POST', uri, true);
   req.setRequestHeader("Content-Type", "application/json");
   req.send(JSON.stringify(tache));
}

// Demande la suppression d'une tâche par XHR
function deleteTache(id) {
   const req = new XMLHttpRequest();
   req.onload = function () {
      if (this.status === 200) {
         // Supprime la tache du tableau et rafraichit le visuel
         for (let t = taches.length - 1; t >= 0; t--) {
            if (taches[t].id === id) taches.splice(t, 1);
         }
         afficherTaches();
      }
      else {
         console.log("erreur requête DELETE : " + this.status + " " + this.statusText);
      }
   };
   req.open('DELETE', uri + "/" + id, true);
   req.send(null);
}

// Envoie une tâche modifiée par XHR
function putTache(tache) {
   const req = new XMLHttpRequest();
   req.onload = function () {
      if (this.status === 200) {
         // Met à jour la tache dans le tableau et rafraichit le visuel
         for (let t = 0; t < taches.length; t++) {
            if (taches[t].id === tache.id) {
               // NB/ Id et date de création ne sont pas modifiables
               taches[t].dateEcheance = tache.dateEcheance;
               taches[t].priorite = tache.priorite;
               taches[t].terminee = tache.terminee;
               taches[t].description = tache.description;
            }
         }
         afficherTaches();
         afficherFormulaire(false);
      }
      else {
         console.log("erreur requête PUT : " + this.status + " " + this.statusText);
      }
   };
   req.open('PUT', uri + "/" + tache.id, true);
   req.setRequestHeader("Content-Type", "application/json");
   req.send(JSON.stringify(tache));
}

//#endregion

//#region Fonctions de gestion du tableau des tâches
//---------------------------------------------------

// Affiche les taches dans le tableau HTML
function afficherTaches() {
   // Remplace le corps du tableau HTML par un corps vide
   const tb1 = document.querySelector("#tabTaches tbody");
   const tb2 = document.createElement("tbody");
   tb1.parentElement.replaceChild(tb2, tb1);

   // Crée de nouvelles lignes pour afficher les données reçues
   for (let t = 0; t < taches.length; t++) {
      const ligne = creerLignePourTache(taches[t]);
      tb2.appendChild(ligne);
   }

   // Affiche le nombre de tâches total et le nombre de tâches non terminées
   document.getElementById("nbTaches").textContent = taches.length;
   document.getElementById("nbTachesNonTerm").textContent = taches.filter(t => !t.terminee).length;
}

// Créer une ligne de tableau pour la tâche passée en paramètre
function creerLignePourTache(tache) {
   const ligne = document.createElement("tr");

   ligne.innerHTML =
      `<td>${tache.id}</td>
      <td>${tache.dateCreation.toLocaleDateString()}</td>
      <td>${tache.dateEcheance < MAXDATE ? tache.dateEcheance.toLocaleDateString() : ""}</td>
      <td>${tache.priorite}</td>
      <td>${tache.terminee ? '&check;' : ''}</td>
      <td>${tache.description}</td>
      <td><span class='action'>&#x1F589;</span></td>               
      <td><span class='action'>&#x1F5D1;</span></td>`;

   // Affecte les actions aux boutons d'édition et de suppression
   const sp = ligne.querySelectorAll("td>span");
   sp[0].onclick = function () { initFormulaire(tache.id); };
   sp[1].onclick = function () { deleteTache(tache.id); };

   return ligne;
}
//#endregion

//#region Fonctions de gestion du formulaire d'édition de tâche
//-------------------------------------------------------------

// Affiche ou masque le formulaire d'édition de tâche
function afficherFormulaire(visible) {
   document.getElementById("detail").style.display = visible ? "inline-block" : "none";
}

// Initialise le formulaire d'édition de tâche
// idTache > 0 si on édite une tâche existante, <0 si on crée une nouvelle tâche
function initFormulaire(idTache) {
   // Affiche le formulaire
   afficherFormulaire(true);

   let tache;

   // Récupère la tâche existante (édition) ou en crée une nouvelle (ajout)
   if (idTache > 0) {
      // Récupère la tâche à éditer à partir de son id
      tache = taches.filter(item => item.id == idTache)[0];
   }
   else {
      // Détermine le plus grand id parmi les tâches existantes
      let max = 0;
      if (taches.length > 0)
         max = taches.reduce((m, x) => Math.max(m, x.id), 0);

      // Créer une nouvelle tâche
      tache = {
         id: max + 1,
         dateCreation: new Date(),
         dateEcheance: "",
         priorite: 1,
         terminee: false,
         description: ""
      };
   }

   // Initialise les champs du formulaire avec la tâche
   document.getElementById("idTache").value = tache.id;
   document.getElementById("dateCreation").value = tache.dateCreation.toStandardFormat();
   document.getElementById("dateEcheance").value = idTache > 0 ? tache.dateEcheance.toStandardFormat() : "";
   document.getElementById("priorite").value = tache.priorite;
   document.getElementById("terminee").checked = tache.terminee;
   document.getElementById("description").value = tache.description;

   // Affecte le traitement à lancer à la soumission du formulaire
   document.getElementById("formEdit").onsubmit = function () {
      soumettreFormulaire(idTache);
   };
}

// Traitement à effectuer à la soumission du formulaire
function soumettreFormulaire(idTache) {
   // Supprime le message d'erreur éventuellement affiché
   let pErreur = document.getElementById("messageErreur");
   pErreur.textContent = "";

   try {
      // Lance la vérification de la tâche saisie, qui peut lever une exception
      let tache = verifierTacheSaisie();
      // Envoie la tâche à l'API
      if (idTache > 0)
         putTache(tache);
      else
         postTache(tache);
   }
   catch (e) {
      // Affiche l'erreur au bas du formulaire
      pErreur.textContent = e.message;
   }
}

// Vérifie si la tâche saisie est valide
// Renvoie la tâche si c'est le cas
// Lève une exception si ce n'est pas le cas
function verifierTacheSaisie() {
   // Récupère un objet tache à partir des valeurs saisies brutes
   const tacheSaisie = getTacheSaisie();

   // Contrôle la cohérence des dates
   if (tacheSaisie.dateEcheance.length > 0 && tacheSaisie.dateEcheance < tacheSaisie.dateCreation) {      
      throw new RangeError("La date d'échéance doit être >= à la date de création");
   }

   // Construit un objet tache à envoyer à l'API
   const heures = "T00:00:00.000";
   let tache = {};
   tache.id = parseInt(tacheSaisie.id, 10);
   tache.dateCreation = new Date(tacheSaisie.dateCreation + heures);
   if (tacheSaisie.dateEcheance.length > 0)
      tache.dateEcheance = new Date(tacheSaisie.dateEcheance + heures);
   else
      tache.dateEcheance = MAXDATE;
   tache.priorite = parseInt(tacheSaisie.priorite, 10);
   tache.terminee = tacheSaisie.terminee;
   tache.description = tacheSaisie.description;

   return tache;
}

// Renvoie un objet Tache à partir des valeurs saisies
function getTacheSaisie() {
   let tache = {
      id: document.getElementById("idTache").value,
      dateCreation: document.getElementById("dateCreation").value,
      dateEcheance: document.getElementById("dateEcheance").value,
      priorite: document.getElementById("priorite").value,
      terminee: document.getElementById("terminee").checked,
      description: document.getElementById("description").value
   };

   return tache;
}

//#endregion