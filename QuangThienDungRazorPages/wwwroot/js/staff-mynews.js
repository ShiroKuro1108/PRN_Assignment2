// Staff My News JavaScript

// View News Function
async function viewNews(newsId) {
    try {
        const response = await fetch(`/Staff/MyNews?handler=NewsDetails&id=${newsId}`);
        const result = await response.json();
        
        if (result.success) {
            const news = result.news;
            
            // Set modal title
            document.getElementById('viewNewsTitle').textContent = news.title || 'Untitled';
            
            // Build content HTML
            const contentHtml = `
                <div class="row">
                    <div class="col-md-6">
                        <h6>Basic Information</h6>
                        <dl class="row">
                            <dt class="col-sm-4">ID:</dt>
                            <dd class="col-sm-8">${news.id}</dd>
                            
                            <dt class="col-sm-4">Category:</dt>
                            <dd class="col-sm-8">${news.category || 'N/A'}</dd>
                            
                            <dt class="col-sm-4">Status:</dt>
                            <dd class="col-sm-8">
                                <span class="badge ${news.status === 'Active' ? 'bg-success' : 'bg-secondary'}">
                                    ${news.status}
                                </span>
                            </dd>
                            
                            <dt class="col-sm-4">Source:</dt>
                            <dd class="col-sm-8">${news.source || 'N/A'}</dd>
                        </dl>
                    </div>
                    
                    <div class="col-md-6">
                        <h6>Timeline</h6>
                        <dl class="row">
                            <dt class="col-sm-4">Created:</dt>
                            <dd class="col-sm-8">${news.createdDate || 'N/A'}</dd>
                            
                            <dt class="col-sm-4">Modified:</dt>
                            <dd class="col-sm-8">${news.modifiedDate || 'Not modified'}</dd>
                            
                            <dt class="col-sm-4">Created By:</dt>
                            <dd class="col-sm-8">${news.createdBy || 'N/A'}</dd>
                            
                            <dt class="col-sm-4">Updated By:</dt>
                            <dd class="col-sm-8">${news.updatedBy || 'N/A'}</dd>
                        </dl>
                    </div>
                </div>
                
                <hr>
                
                <div class="mb-3">
                    <h6>Headline</h6>
                    <p class="text-muted">${news.headline || 'No headline'}</p>
                </div>
                
                <div class="mb-3">
                    <h6>Content</h6>
                    <div class="border p-3 bg-light" style="max-height: 300px; overflow-y: auto;">
                        ${news.content ? news.content.replace(/\n/g, '<br>') : 'No content'}
                    </div>
                </div>
                
                ${news.tags && news.tags.length > 0 ? `
                <div class="mb-3">
                    <h6>Tags</h6>
                    ${news.tags.map(tag => `<span class="badge bg-info me-1">${tag}</span>`).join('')}
                </div>
                ` : ''}
            `;
            
            // Set modal content
            document.getElementById('viewNewsContent').innerHTML = contentHtml;
            
            // Show modal
            new bootstrap.Modal(document.getElementById('viewNewsModal')).show();
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while loading the news article.');
    }
}

// Utility Functions
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
    // Auto-dismiss any existing alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            if (alert.parentNode) {
                alert.remove();
            }
        }, 5000);
    });
});
