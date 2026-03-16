# Virtual Restitution

![Unity](https://img.shields.io/badge/Unity-6000.3.8f1-black?style=flat-square&logo=unity)
![Render Pipeline](https://img.shields.io/badge/Render%20Pipeline-URP-blue?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-PCVR%20%7C%20Meta%20Quest-lightgrey?style=flat-square)
![License](https://img.shields.io/badge/License-Apache%202.0-blue?style=flat-square)
![License](https://img.shields.io/badge/License-Attribution%20Required-orange?style=flat-square)

**Virtual Restitution** is an interactive Virtual Reality experience designed to critique the colonial display of African artifacts. By moving through digital space, the user participates in the symbolic "repatriation" of objects—shifting them from sterile museum vitrines back to their living, functional, and spiritual contexts.

---

## 📂 Table of Contents
* [The Experience: A Narrative of Return](#️-the-experience-a-narrative-of-return)
* [Impressions](#️-impressions)
* [Technical Details](#-technical-details)
    * [Installation & Setup](#installation--setup)
    * [Features & Pipeline](#features--pipeline)
    * [VR Controls & Locomotion](#vr-controls--locomotion)
    * [Building the project](#building-the-project)
* [License & Attribution](#-license--attribution)
* [Attributions](#️-attributions)

---

## 🏛️ The Experience: A Narrative of Return

The experience is structured as a psychological and spatial journey through three distinct phases:

* **The Sterile Void:** A cold, foggy museum room. Artifacts are trapped in glass boxes, stripped of their history, highlighting the "wrongness" of these objects in a Western gallery.
* **The Archive & The Transit:** An overwhelming archive of inaccessible history leads to a shipping container. This serves as a "reverse passage"—symbolizing the physical return of the objects to their origins.
* **Contextual Restoration:** The journey ends in a warm Ghanaian evening. In a traditional setting, characters emerge to use the artifacts, restoring their functional and spiritual life through light, sound, and communal harmony.

---

## 🖼️ Impressions

| | | |
| :---: | :---: | :---: |
| <img src="https://github.com/user-attachments/assets/f8169578-de50-4534-b44c-767d63cc6976" height="200"> | <img src="https://github.com/user-attachments/assets/bcc231ac-fd31-4d75-a141-ce6af4f68c76" height="200"> | <img src="https://github.com/user-attachments/assets/1a631af1-b96a-4341-a833-df8745aee5ff" height="200"> |
| <img src="https://github.com/user-attachments/assets/6fc2da79-ae60-46d6-863a-33ef5767c674" height="200"> | <img src="https://github.com/user-attachments/assets/560a844c-b930-44ac-8d04-400053e7d32f" height="200"> | <img src="https://github.com/user-attachments/assets/e27e7641-c0d9-470d-8dd2-8c10c64dd7aa" height="200"> |
| <img src="https://github.com/user-attachments/assets/3a7eb5eb-d121-42e5-8a25-80277518ef18" height="200"> | | |

---

## 🛠 Technical Details

Virtual Restitution was developed using **Unity 6 (Version 6000.3.8f1)** and utilizes the **Universal Render Pipeline (URP)** to handle its lighting and atmospheric effects, integrated with **OpenXR** for virtual reality support.

### Installation & Setup
> [!WARNING]  
> This repository uses **Git LFS** (Large File Storage) for high-resolution textures and 3D models. You must have Git LFS installed before cloning, otherwise, your assets will be broken.

1. Install [Git LFS](https://git-lfs.github.com/).
2. Run `git lfs install` in your terminal.
3. Clone the repository: `git clone https://github.com/JI20/virtual-restitution/`
4. Open the project in Unity 6000.3.8f1. Load the **'Main'** Scene (found within the `Scenes` folder) and hit Play. Please ensure no other scenes are loaded simultaneously.

### Features & Pipeline
* **Scene Management:** We implemented a custom runtime `SceneLoader` script that handles the additive loading and unloading of the correct environments based on player progression.
* **Lighting & Post-Processing:** The environments rely heavily on **Baked Lighting** to achieve smooth, realistic shadows where possible, combined with several active post-processing effects to transition the atmosphere from cold and sterile to warm and familiar.
* **Shaders:** The spiritual/holographic characters are driven by a custom **Fresnel Effect Shader Graph**.
* **Audio:** The project uses **Spatial 3D Audio** to ground the user in the narrative and guide them toward interactions.

### VR Controls & Locomotion
For the best experience, **standing up** is highly recommended. 
* **Movement:** Smooth locomotion using the **Left Thumbstick**. While room-scale physical walking (a few steps) is supported, the environments are designed to be navigated via thumbstick movement.
* **Looking:** Standard 6DOF headset tracking.
* **Note:** The Right Controller is not actively needed to navigate this experience.

#### Desktop Fallback (Non-VR)
To speed up development and for accessibility, we built in support for running the project in a standard desktop mode. Without VR glasses, you can navigate the environment using **WASD keys** for movement and the **mouse cursor** to look around.

### Building the project
Please note that we do not provide pre-built versions or executables; you will need to build from source. 

While the project is structured to be buildable for either **Windows (PCVR)** or **Android (Native Meta Quest)**, our primary development focus was on creating a high-fidelity **PCVR game** (using Meta's Horizon Link via Link Cable). We did not extensively test or optimize performance for native standalone VR builds on Android/Meta Quest.

---

## 📜 License & Attribution
This project is licensed under the **Apache License 2.0**. 

### 📢 Explicit Requirement for Attribution
If you use, share, modify, or redistribute this project (or any part of it), you **must** provide clear and visible credit to the original creators. 

**Please attribute as follows:**
> *"Virtual Restitution" by [Your Name/Team Name], available at https://github.com/JI20/virtual-restitution/*

If you modify the project, you must include a notice stating that changes were made to the original work.

---

## 🏗️ Attributions

### 🗿 3D Models & Assets
* **Principal Artifact:** [Veranda Post of Enthroned King and Senior Wife](https://cults3d.com/en/3d-model/art/veranda-post-of-enthroned-king-and-senior-wife-opo-ogoga-1910-14) (Opo Ogoga) by **ArtInstituteChicago** ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
* **University of Education, Winneba (UEW) Collection:** Multiple 3D scans of African artifacts provided by **Ebenezer Kow Abraham**. ([Staff Profile & Publications](https://www.uew.edu.gh/artedu/staff/eeabraham/publications))
* **Drums & Instruments:**
    * [African Drum](https://sketchfab.com/3d-models/african-drum-3b09e1d365be4f069417b2f54ba4e7c4) by Inês Freitas ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
    * [Tambour](https://sketchfab.com/3d-models/tambour-991bbeb697004e1cb71da68c46464076) by Musée des Confluences ([CC BY-NC-ND 4.0](https://creativecommons.org/licenses/by-nc-nd/4.0/))
    * [Djembe African Drum Scan](https://sketchfab.com/3d-models/djembe-african-drum-scan-6db07a048621443992a8048dd62c026f) by heritagemuseum ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
* **Environment & Props:**
    * [Classic Shipping Container](https://sketchfab.com/3d-models/classic-shipping-container-freegameready-772f4be391a245f699c54e6cf157c58d) by seth_steward ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
    * [Light Ceiling Lamp](https://sketchfab.com/3d-models/light-ceiling-lamp-15mb-eaaab08eac454e59840fa4818fc55ff3) by Al-Ansari ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
    * [Box Package](https://sketchfab.com/3d-models/box-package-0335892364814f5d9006542e03e58741) by bit_is ([CC BY 4.0](https://creativecommons.org/licenses/by/4.0/))
* **Animations:** Character movements provided by [Mixamo](https://www.mixamo.com/).

### 📦 Unity Asset Store (Extension Asset License)
* **Environment:** [Pandazole Nature Environment (Low Poly)](https://assetstore.unity.com/packages/3d/environments/pandazole-nature-environment-low-poly-pack-212621) by Pandazole
* **Environment:** [LowPoly Environment Pack](https://assetstore.unity.com/packages/3d/environments/landscapes/lowpoly-environment-pack-99479) by The_Mustacho
* **Skyboxes:** * [AllSky Free - 10 Skybox Set](https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-free-10-sky-skybox-set-146014) by RPGWhitelock
    * [Free HDR Skyboxes Pack](https://assetstore.unity.com/packages/2d/textures-materials/sky/free-hdr-skyboxes-pack-175525) by Pro_At_Art

### 🎞️ Historical Documentation & Imagery
* **Archival Photos:**
    * [Burning of Coomassie (Kumasi), 1874](https://commons.wikimedia.org/wiki/File:Burning_of_Coomassie.jpg) (Public Domain)
    * [Interior of Oba's compound, Benin City, 1897](https://fr.wikipedia.org/wiki/Fichier:Interior_of_Oba%27s_compound_burnt_during_siege_of_Benin_City,_1897.jpg) (Public Domain)
    * [Femmes Galoa de Talagouga du Gabon](https://www.amazinggabon.com/fr/portfolio/galoa/attachment/femmes-galoa-de-talagouga-du-gabon/) via Amazing Gabon
* **Journalism & Research References:**
    * [The Decolonization of African Art](https://theswisstimes.ch/fr/the-decolonization-of-african-art-in-swiss-museums/) - The Swiss Times
    * [Restitution of Royal Objects to Ghana](https://www.voaafrique.com/a/un-mus%C3%A9e-am%C3%A9ricain-restitue-des-objets-royaux-au-ghana-pill%C3%A9s-pendant-la-colonisation/7479729.html) - VOA Afrique
    * [The Brutish Museums & Benin Bronzes](https://edition.cnn.com/style/article/brutish-museums-benin-bronzes) - CNN Style

### 🎥 Educational Multimedia
* [Asante Traditional Buildings](https://www.youtube.com/watch?v=wjJWRyEtzs8) – The Met
* [Architecture of Asante traditional buildings](https://www.youtube.com/watch?v=nSMT7XGzRno) – JoyNews
* [The Golden Stool - History of Africa](https://www.youtube.com/watch?v=_KKnpSnXRxo) – BBC News Africa
* [Ashanti Women: Queens of Golden Legacy](https://www.youtube.com/watch?v=xawEDZ1P0mQ) – AFRO VERSITY
* [Exploring Ghana's Diverse Culture](https://www.youtube.com/watch?v=7r-WQeHnRF0) – News Central TV
* [10 African Traditional Dance Styles](https://www.youtube.com/watch?v=3sgaM19f4xU) – ohAFRO

### 🎨 Textures & Materials
* All environment textures sourced from [ambientCG](https://ambientcg.com/) and [Poly Haven](https://polyhaven.com/license) under **CC0 (Public Domain)**.
