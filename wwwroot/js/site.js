// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", () => {
    const revealItems = document.querySelectorAll(
        ".home-main-hero, .category-page, .category-icon-card, .action-big-card, .home-stats div, .home-carousel-section, .mini-card, .media-card, .form-shell, .cinematic-details"
    );

    revealItems.forEach((item) => {
        item.classList.add("reveal-item");
    });

    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add("revealed");
                }
            });
        },
        {
            threshold: 0.12
        }
    );

    revealItems.forEach((item) => observer.observe(item));
});

document.addEventListener("DOMContentLoaded", () => {
    const skeleton = document.querySelector(".page-loader");

    if (skeleton) {
        setTimeout(() => {
            skeleton.classList.add("hide-loader");
        }, 450);
    }
});
function toggleMobileMenu() {
    const menu = document.querySelector(".nav-menu");
    menu.classList.toggle("open");
}
document.querySelectorAll(".nav-menu a").forEach(link => {
    link.addEventListener("click", () => {
        document.querySelector(".nav-menu")?.classList.remove("open");
    });
});
function openPosterPreview(src, title) {
    const modal = document.getElementById("posterModal");
    const image = document.getElementById("posterModalImage");
    const caption = document.getElementById("posterModalTitle");

    image.src = src;
    image.alt = title;
    caption.textContent = title;

    modal.classList.add("active");
}

function closePosterPreview() {
    document.getElementById("posterModal").classList.remove("active");
}
const uploadZone = document.getElementById("uploadZone");
const coverInput = document.getElementById("coverInput");
const previewImage = document.getElementById("previewImage");

if (uploadZone && coverInput) {

    const uploadZone = document.getElementById("uploadZone");
    const coverInput = document.getElementById("coverInput");
    const previewImage = document.getElementById("previewImage");

    if (uploadZone && coverInput && previewImage) {
        ["dragenter", "dragover"].forEach(eventName => {
            uploadZone.addEventListener(eventName, e => {
                e.preventDefault();
                uploadZone.classList.add("dragover");
            });
        });

        ["dragleave", "drop"].forEach(eventName => {
            uploadZone.addEventListener(eventName, e => {
                e.preventDefault();
                uploadZone.classList.remove("dragover");
            });
        });

        uploadZone.addEventListener("drop", e => {
            const files = e.dataTransfer.files;

            if (files.length > 0) {
                coverInput.files = files;
                showCoverPreview(files[0]);
            }
        });

        coverInput.addEventListener("change", () => {
            if (coverInput.files.length > 0) {
                showCoverPreview(coverInput.files[0]);
            }
        });
    }

    function showCoverPreview(file) {
        if (!file || !file.type.startsWith("image/")) return;

        const reader = new FileReader();

        reader.onload = e => {
            previewImage.src = e.target.result;
            previewImage.classList.add("active");
        };

        reader.readAsDataURL(file);
    }

    coverInput.addEventListener("change", () => {
        const file = coverInput.files[0];

        if (file) {
            const reader = new FileReader();

            reader.onload = e => {
                previewImage.src = e.target.result;
                previewImage.classList.add("active");
            };

            reader.readAsDataURL(file);
        }
    });
}
const liveSearchInput = document.getElementById("liveSearchInput");

if (liveSearchInput) {
    let searchTimer;

    liveSearchInput.addEventListener("input", () => {
        clearTimeout(searchTimer);

        searchTimer = setTimeout(() => {
            liveSearchInput.closest("form").submit();
        }, 600);
    });
}
function animateHeart(button) {
    button.classList.add("heart-clicked");

    setTimeout(() => {
        button.classList.remove("heart-clicked");
    }, 450);
}
function toggleMobileFilters() {
    const filters = document.getElementById("mobileFilters");

    if (filters) {
        filters.classList.toggle("open");
    }
}
function toggleMobileFilters() {

    const filters = document.getElementById("mobileFilters");
    const arrow = document.getElementById("filterArrow");

    if (filters) {
        filters.classList.toggle("open");
    }

    if (arrow) {
        arrow.classList.toggle("open");
    }
}
document.addEventListener("submit", async function (e) {
    const form = e.target.closest(".ajax-favorite-form");

    if (!form) return;

    e.preventDefault();
    e.stopPropagation();

    const button = form.querySelector(".favorite-badge");
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;

    try {
        const response = await fetch(form.action, {
            method: "POST",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (!response.ok) return;

        const result = await response.json();

        if (result.success) {
            button.classList.toggle("active", result.isFavorite);
            button.classList.add("heart-pop");

            setTimeout(() => {
                button.classList.remove("heart-pop");
            }, 350);
        }
    } catch {
        console.log("Favorite toggle failed");
    }
});