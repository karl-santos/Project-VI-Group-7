const params = new URLSearchParams(window.location.search);
let gameId = params.get("gameId");
const runId = params.get("runId");

function showError(msg) {
    $("#error").text(msg).show();
}

function hideError(msg) {
    $("#error").hide().text("");
}

function showNotFound() {
    $("#content").hide();
    $("#notFound").show();
}

function showContent() {
    $("#notFound").hide();
    $("#content").show();
}

function isValidTime(s) {
    s = ((s || "")).trim();
    return /^\d{2}:\d{2}$/.test(s) || /^\d{2}:\d{2}:\d{2}$/.test(s);
}


//if id is known, go back to leaderboard, otherwose go to the games page
function setBackLink() {
    if (gameId) $("#backLink").attr("href", `Leaderboard.html?gameId=${gameId}`);
    else $("#backLink").attr("href", `/games.html`);
}


function fillRun(run) {
    $("#gameId").val(run.gameId ?? "");
    $("#runId").val(run.id ?? "");
    $("#playerName").val(run.playerName ?? "");
    $("#category").val(run.category ?? "");
    $("#time").val(run.time ?? "");
    $("#videoUrl").val(run.videoUrl ?? "");
    $("#notes").val(run.notes ?? "");

    if (run.submittedAt) {
        $("#submittedAt").val(new Date(run.submittedAt).toLocaleString());
    } else {
        $("#submittedAt").val("");
    }

    if (run.videoUrl) {
        $("#videoLink")
            .attr("href", run.videoUrl)
            .show();
    } else {
        $("#videoLink").hide();
    }
}

function loadComments() {
    if (!gameId || !runId) return;

    $.ajax({
        url: `/api/games/${gameId}/runs/${runId}/comments`,
        method: "GET",
        success: function (comments) {
            const list = $("#commentList");
            list.empty();

            if (!comments || comments.length === 0) {
                list.append(`<p class="hint">No comments yet. Be the first!</p>`);
                return;
            }

            comments.forEach(c => {
                const when = c.createdAt ? new Date(c.createdAt).toLocaleString() : "";
                list.append(`
          <div class="comment-card">
            <div class="comment-head">
              <strong>${c.username || "Anonymous"}</strong>
              <span class="hint">${when}</span>
            </div>
            <div>${c.content || ""}</div>
          </div>
        `);
            });
        },
        error: function () {
            $("#commentList").html(`<p class="hint">Could not load comments.</p>`);
        }
    });
}

function loadRunWithKnownGameId() {
    setBackLink();

    $.ajax({
        url: `/api/games/${gameId}/runs/${runId}`,
        method: "GET",
        success: function (run) {
            showContent();
            hideError();
            fillRun(run);
            loadComments();
        },
        error: function () {
            showNotFound();
        }
    });
}

// fallback when the leaderboard didn’t include gameId in URL
function findGameIdForRunId() {

    $.ajax({
        url: `/api/games`,
        method: "GET",
        success: function (games) {
            if (!games || games.length === 0) {
                showNotFound();
                return;
            }

            let found = false;
            let pending = games.length;

            games.forEach(g => {
                $.ajax({
                    url: `/api/games/${g.id}/runs`,
                    method: "GET",
                    success: function (runs) {
                        if (found) return;

                        const match = (runs || []).find(r => String(r.id) === String(runId));
                        if (match) {
                            found = true;
                            gameId = String(g.id);
                            setBackLink();

                            loadRunWithKnownGameId();
                        }
                    },
                    complete: function () {
                        pending--;
                        if (pending === 0 && !found) {
                            showNotFound();
                        }
                    }
                });
            });
        },
        error: function () {
            showNotFound();
        }
    });
}

// update
$("#runForm").on("submit", function (e) {
    e.preventDefault();
    hideError();

    if (!runId) {
        showError("Missing runId in URL.");
        return;
    }
    if (!gameId) {
        showError("Missing gameId (could not resolve the game for this run).");
        return;
    }

    const time = $("#time").val().trim();
    const videoUrl = $("#videoUrl").val().trim();
    const notes = ($("#notes").val() || "").trim();

    if (!isValidTime(time)) {
        showError("Time must be in  MM:SS or HH:MM:SS format (example: 04:04 or 04:04:04).");
        return;
    }

    const payload = {
        time: time,
        videoUrl: videoUrl || null,
        notes: notes || null
    };

    $.ajax({
        url: `/api/games/${gameId}/runs/${runId}`,
        method: "PATCH",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function () {
            hideError();

            loadRunWithKnownGameId();
        },
        error: function (xhr) {
            showError(xhr.responseText || "Update failed.");
        }
    });
});

// delete
$("#deleteBtn").on("click", function () {
    hideError();

    if (!runId) {
        showError("Missing runId in URL.");
        return;
    }
    if (!gameId) {
        showError("Missing gameId (could not resolve the game for this run).");
        return;
    }

    if (!confirm("Delete this run? This cannot be undone.")) return;

    $.ajax({
        url: `/api/games/${gameId}/runs/${runId}`,
        method: "DELETE",
        success: function () {
            window.location = `Leaderboard.html?gameId=${gameId}`;
        },
        error: function (xhr) {
            showError(xhr.responseText || "Delete failed.");
        }
    });
});

// post
$("#commentForm").on("submit", function (e) {
    e.preventDefault();
    hideError();

    if (!runId) {
        showError("Missing runId in URL.");
        return;
    }
    if (!gameId) {
        showError("Missing gameId (could not resolve the game for this run).");
        return;
    }

    const username = $("#username").val().trim();
    const content = $("#commentContent").val().trim();

    if (!username || !content) {
        showError("Username and comment are required.");
        return;
    }

    const payload = { username, content };

    $.ajax({
        url: `/api/games/${gameId}/runs/${runId}/comments`,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function () {
            $("#commentContent").val("");
            $("#username").val("");
            loadComments();
        },
        error: function (xhr) {
            showError(xhr.responseText || "Comment failed.");
        }
    });
});

(function init() {
    if (!runId) {
        showNotFound();
        return;
    }

    if (gameId) {
        loadRunWithKnownGameId();
    } else {

        setBackLink();
        findGameIdForRunId();
    }
})();