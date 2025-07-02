// Staff News Management JavaScript with SignalR

// SignalR Connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/newsHub")
    .build();

// Start SignalR connection
connection.start().then(function () {
    console.log("SignalR Connected");
}).catch(function (err) {
    console.error("SignalR Connection Error: ", err.toString());
});

// SignalR Event Handlers
connection.on("NewsCreated", function (newsId, title, author) {
    showSignalRNotification('success', `New article "${title}" created by ${author}`, newsId);
});

connection.on("NewsUpdated", function (newsId, title, author) {
    showSignalRNotification('info', `Article "${title}" updated by ${author}`, newsId);
});

connection.on("NewsDeleted", function (newsId, title, author) {
    showSignalRNotification('warning', `Article "${title}" deleted by ${author}`, newsId);
    // Remove the row from table if it exists
    const row = document.querySelector(`tr[data-news-id="${newsId}"]`);
    if (row) {
        row.remove();
    }
});

// Create News Form Handler
document.getElementById('createNewsForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const selectedTags = Array.from(document.getElementById('createNewsTags').selectedOptions)
                             .map(option => parseInt(option.value));
    
    const formData = {
        title: document.getElementById('createNewsTitle').value,
        headline: document.getElementById('createNewsHeadline').value,
        content: document.getElementById('createNewsContent').value,
        source: document.getElementById('createNewsSource').value,
        categoryId: document.getElementById('createNewsCategory').value ? 
                   parseInt(document.getElementById('createNewsCategory').value) : null,
        isActive: document.getElementById('createNewsStatus').checked,
        tagIds: selectedTags
    };

    try {
        const response = await fetch('/Staff/News?handler=Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            document.getElementById('createNewsForm').reset();
            bootstrap.Modal.getInstance(document.getElementById('createNewsModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while creating the news article.');
    }
});

// Edit News Form Handler
document.getElementById('editNewsForm').addEventListener('submit', async function(e) {
    e.preventDefault();
    
    const selectedTags = Array.from(document.getElementById('editNewsTags').selectedOptions)
                             .map(option => parseInt(option.value));
    
    const formData = {
        id: document.getElementById('editNewsId').value,
        title: document.getElementById('editNewsTitle').value,
        headline: document.getElementById('editNewsHeadline').value,
        content: document.getElementById('editNewsContent').value,
        source: document.getElementById('editNewsSource').value,
        categoryId: document.getElementById('editNewsCategory').value ? 
                   parseInt(document.getElementById('editNewsCategory').value) : null,
        isActive: document.getElementById('editNewsStatus').checked,
        tagIds: selectedTags
    };

    try {
        const response = await fetch('/Staff/News?handler=Update', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify(formData)
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            bootstrap.Modal.getInstance(document.getElementById('editNewsModal')).hide();
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while updating the news article.');
    }
});

// Edit News Function
async function editNews(newsId) {
    try {
        const response = await fetch(`/Staff/News?handler=News&id=${newsId}`);
        const result = await response.json();
        
        if (result.success) {
            const news = result.news;
            document.getElementById('editNewsId').value = news.id;
            document.getElementById('editNewsTitle').value = news.title || '';
            document.getElementById('editNewsHeadline').value = news.headline || '';
            document.getElementById('editNewsContent').value = news.content || '';
            document.getElementById('editNewsSource').value = news.source || '';
            document.getElementById('editNewsCategory').value = news.categoryId || '';
            document.getElementById('editNewsStatus').checked = news.isActive;
            
            // Set selected tags
            const tagSelect = document.getElementById('editNewsTags');
            Array.from(tagSelect.options).forEach(option => {
                option.selected = news.tagIds.includes(parseInt(option.value));
            });
            
            new bootstrap.Modal(document.getElementById('editNewsModal')).show();
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while loading the news article.');
    }
}

// Delete News Function
async function deleteNews(newsId, newsTitle) {
    if (!confirm(`Are you sure you want to delete the news article "${newsTitle}"? This action cannot be undone.`)) {
        return;
    }

    try {
        const response = await fetch('/Staff/News?handler=Delete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ id: newsId })
        });

        const result = await response.json();
        
        if (result.success) {
            showAlert('success', result.message);
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while deleting the news article.');
    }
}

// SignalR Notification Function
function showSignalRNotification(type, message, newsId) {
    const notificationDiv = document.createElement('div');
    notificationDiv.className = `alert alert-${type} alert-dismissible fade show`;
    notificationDiv.innerHTML = `
        <strong>Real-time Update:</strong> ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.getElementById('signalr-notifications');
    container.appendChild(notificationDiv);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (notificationDiv.parentNode) {
            notificationDiv.remove();
        }
    }, 5000);
}

// Utility Functions
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
}

function showAlert(type, message) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    const container = document.querySelector('.container-fluid');
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

// Initialize page
document.addEventListener('DOMContentLoaded', function() {
    // Add anti-forgery token if not exists
    if (!document.querySelector('input[name="__RequestVerificationToken"]')) {
        const tokenInput = document.createElement('input');
        tokenInput.type = 'hidden';
        tokenInput.name = '__RequestVerificationToken';
        tokenInput.value = document.querySelector('meta[name="csrf-token"]')?.getAttribute('content') || '';
        document.body.appendChild(tokenInput);
    }
});
