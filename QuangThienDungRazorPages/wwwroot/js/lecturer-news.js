// Lecturer News JavaScript

// View News Details Function
async function viewNewsDetails(newsId) {
    try {
        const response = await fetch(`/Lecturer/News?handler=NewsDetails&id=${newsId}`);
        const result = await response.json();
        
        if (result.success) {
            const news = result.news;
            
            // Set modal title
            document.getElementById('newsDetailsTitle').textContent = news.title || 'Untitled';
            
            // Build content HTML
            const contentHtml = `
                <div class="row mb-3">
                    <div class="col-md-6">
                        <h6>Article Information</h6>
                        <dl class="row">
                            <dt class="col-sm-4">ID:</dt>
                            <dd class="col-sm-8">${news.id}</dd>
                            
                            <dt class="col-sm-4">Category:</dt>
                            <dd class="col-sm-8">${news.category || 'Uncategorized'}</dd>
                            
                            <dt class="col-sm-4">Source:</dt>
                            <dd class="col-sm-8">${news.source || 'N/A'}</dd>
                            
                            <dt class="col-sm-4">Author:</dt>
                            <dd class="col-sm-8">${news.createdBy || 'Unknown'}</dd>
                        </dl>
                    </div>
                    
                    <div class="col-md-6">
                        <h6>Publication Details</h6>
                        <dl class="row">
                            <dt class="col-sm-4">Published:</dt>
                            <dd class="col-sm-8">${news.createdDate || 'N/A'}</dd>
                            
                            <dt class="col-sm-4">Last Updated:</dt>
                            <dd class="col-sm-8">${news.modifiedDate || 'Not modified'}</dd>
                            
                            <dt class="col-sm-4">Status:</dt>
                            <dd class="col-sm-8">
                                <span class="badge bg-success">Active</span>
                            </dd>
                        </dl>
                    </div>
                </div>
                
                <hr>
                
                <div class="mb-3">
                    <h6>Headline</h6>
                    <p class="lead">${news.headline || 'No headline'}</p>
                </div>
                
                <div class="mb-3">
                    <h6>Content</h6>
                    <div class="border p-3 bg-light" style="max-height: 400px; overflow-y: auto;">
                        ${news.content ? news.content.replace(/\n/g, '<br>') : 'No content available'}
                    </div>
                </div>
                
                ${news.tags && news.tags.length > 0 ? `
                <div class="mb-3">
                    <h6>Tags</h6>
                    <div>
                        ${news.tags.map(tag => `<span class="badge bg-secondary me-1">${tag}</span>`).join('')}
                    </div>
                </div>
                ` : ''}
            `;
            
            // Set modal content
            document.getElementById('newsDetailsContent').innerHTML = contentHtml;
            
            // Show modal
            new bootstrap.Modal(document.getElementById('newsDetailsModal')).show();
        } else {
            showAlert('danger', result.message);
        }
    } catch (error) {
        showAlert('danger', 'An error occurred while loading the news article details.');
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

// Search functionality enhancement
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

    // Add keyboard shortcut for search (Ctrl+F or Cmd+F)
    document.addEventListener('keydown', function(e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
            e.preventDefault();
            const searchInput = document.querySelector('input[name="searchTerm"]');
            if (searchInput) {
                searchInput.focus();
                searchInput.select();
            }
        }
    });

    // Add real-time search suggestion (optional enhancement)
    const searchInput = document.querySelector('input[name="searchTerm"]');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            // Could implement live search suggestions here
            // For now, just provide visual feedback
            if (this.value.length > 0) {
                this.classList.add('border-primary');
            } else {
                this.classList.remove('border-primary');
            }
        });
    }
});
