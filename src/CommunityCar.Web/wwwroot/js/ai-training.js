/**
 * AI Training Management
 * Handles model training, retraining, deletion, and exports.
 */
class AITrainingManager {
    constructor() {
        this.init();
    }

    init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => this.setup());
        } else {
            this.setup();
        }
    }

    setup() {
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
        this.setupEventHandlers();
        this.setupOutsideClickHandlers();
    }

    setupEventHandlers() {
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('[data-action]');
            if (!target) return;

            const action = target.dataset.action;
            const model = target.dataset.model;
            const date = target.dataset.date;

            // Handle actions
            switch (action) {
                case 'start-training':
                    this.startTraining();
                    break;
                case 'retrain-model':
                    this.retrainModel(model, target);
                    break;
                case 'toggle-options':
                    event.stopPropagation();
                    this.toggleModelOptions(model);
                    break;
                case 'view-details':
                    event.stopPropagation();
                    this.viewModelDetails(model);
                    break;
                case 'edit-settings':
                    event.stopPropagation();
                    this.editModelSettings(model);
                    break;
                case 'export-model':
                    event.stopPropagation();
                    this.exportModel(model);
                    break;
                case 'delete-model':
                    event.stopPropagation();
                    this.deleteModel(model);
                    break;
                case 'view-training-details':
                    this.viewTrainingDetails(model, date);
                    break;
                case 'download-report':
                    this.downloadTrainingReport(model, date);
                    break;
            }
        });
    }

    setupOutsideClickHandlers() {
        document.addEventListener('click', (event) => {
            const dropdowns = document.querySelectorAll('[id^="options-"]');
            dropdowns.forEach(dropdown => {
                const modelName = dropdown.id.replace('options-', '');
                const toggleButton = document.querySelector(`button[data-action="toggle-options"][data-model="${modelName}"]`);

                if (!dropdown.contains(event.target) && (!toggleButton || !toggleButton.contains(event.target))) {
                    dropdown.classList.add('hidden');
                }
            });
        });
    }

    async startTraining() {
        if (!window.AlertModal) {
            if (!confirm('Are you sure you want to start a new training session? This will queue training for all available models.')) return;
        } else {
            const confirmed = await new Promise(resolve => {
                window.AlertModal.confirm(
                    'Are you sure you want to start a new training session? This will queue training for all available models.',
                    'Start Training Session',
                    resolve
                );
            });
            if (!confirmed) return;
        }

        try {
            const response = await fetch('/Dashboard/AIManagement/StartTraining', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                }
            });

            const result = await response.json();

            if (result.success) {
                this.notify('Training session started successfully!', 'success');
                setTimeout(() => location.reload(), 1500);
            } else {
                this.notify(result.message || 'Failed to start training session', 'error');
            }
        } catch (error) {
            console.error('Error starting training:', error);
            this.notify('Failed to start training session', 'error');
        }
    }

    async retrainModel(modelName, event) {
        if (!window.AlertModal) {
            if (!confirm(`Are you sure you want to retrain the ${modelName} model?`)) return;
        } else {
            const confirmed = await new Promise(resolve => {
                window.AlertModal.confirm(
                    `Are you sure you want to retrain the ${modelName} model? This will queue a new training job.`,
                    'Retrain Model',
                    resolve
                );
            });
            if (!confirmed) return;
        }

        const button = event.target.closest('button');
        const originalContent = button.innerHTML;

        try {
            button.disabled = true;
            button.innerHTML = '<i data-lucide="loader-2" class="w-3 h-3 mr-1 animate-spin"></i>Queuing...';
            if (typeof lucide !== 'undefined') lucide.createIcons();

            const response = await fetch('/Dashboard/AIManagement/RetrainModel', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ modelName: modelName })
            });

            const result = await response.json();

            if (result.success) {
                this.notify(`${modelName} model queued for retraining`, 'success');
                setTimeout(() => location.reload(), 1500);
            } else {
                this.notify(result.message || `Failed to queue ${modelName} for retraining`, 'error');
                button.disabled = false;
                button.innerHTML = originalContent;
                if (typeof lucide !== 'undefined') lucide.createIcons();
            }
        } catch (error) {
            console.error('Error retraining model:', error);
            this.notify(`Failed to queue ${modelName} for retraining`, 'error');
            button.disabled = false;
            button.innerHTML = originalContent;
            if (typeof lucide !== 'undefined') lucide.createIcons();
        }
    }

    toggleModelOptions(modelName, event) {
        if (event) event.stopPropagation();

        const dropdown = document.getElementById(`options-${modelName}`);
        const allDropdowns = document.querySelectorAll('[id^="options-"]');

        allDropdowns.forEach(d => {
            if (d !== dropdown) d.classList.add('hidden');
        });

        dropdown.classList.toggle('hidden');
    }

    viewModelDetails(modelName, event) {
        if (event) event.stopPropagation();
        document.getElementById(`options-${modelName}`).classList.add('hidden');

        if (!window.AlertModal) return;

        const detailsHtml = `
            <div class="space-y-4 text-left">
                <div class="grid grid-cols-2 gap-4">
                    <div>
                        <label class="text-sm font-medium text-muted-foreground uppercase tracking-widest text-[10px]">Model Name</label>
                        <p class="text-sm font-black text-foreground">${modelName}</p>
                    </div>
                    <div>
                        <label class="text-sm font-medium text-muted-foreground uppercase tracking-widest text-[10px]">Status</label>
                        <p class="text-sm font-black text-foreground">Active</p>
                    </div>
                    <div>
                        <label class="text-sm font-medium text-muted-foreground uppercase tracking-widest text-[10px]">Accuracy</label>
                        <p class="text-sm font-black text-foreground">94.2%</p>
                    </div>
                    <div>
                        <label class="text-sm font-medium text-muted-foreground uppercase tracking-widest text-[10px]">Dataset Size</label>
                        <p class="text-sm font-black text-foreground">15,000 samples</p>
                    </div>
                </div>
                <div class="pt-4 border-t border-border/50">
                    <label class="text-sm font-medium text-muted-foreground uppercase tracking-widest text-[10px]">Performance Metrics</label>
                    <div class="mt-3 space-y-2">
                        <div class="flex justify-between items-center text-xs font-bold">
                            <span class="opacity-60">Precision</span>
                            <span class="text-primary">92.1%</span>
                        </div>
                        <div class="flex justify-between items-center text-xs font-bold">
                            <span class="opacity-60">Recall</span>
                            <span class="text-primary">89.7%</span>
                        </div>
                    </div>
                </div>
            </div>
        `;

        window.AlertModal.show({
            type: 'info',
            title: `${modelName} Profile`,
            message: detailsHtml,
            confirmText: 'Acknowledge'
        });
    }

    editModelSettings(modelName, event) {
        if (event) event.stopPropagation();
        document.getElementById(`options-${modelName}`).classList.add('hidden');

        if (!window.AlertModal) return;

        const settingsHtml = `
            <div class="space-y-4 text-left">
                <div>
                    <label class="text-[10px] font-black uppercase tracking-widest text-muted-foreground mb-1.5 block">Learning Rate</label>
                    <input type="number" id="learningRate" class="input w-full" value="0.001" step="0.0001">
                </div>
                <div>
                    <label class="text-[10px] font-black uppercase tracking-widest text-muted-foreground mb-1.5 block">Batch Size</label>
                    <input type="number" id="batchSize" class="input w-full" value="32">
                </div>
            </div>
        `;

        window.AlertModal.show({
            type: 'info',
            title: `Configure ${modelName}`,
            message: settingsHtml,
            showCancel: true,
            confirmText: 'Save Configuration',
            callback: async (confirmed) => {
                if (!confirmed) return;

                try {
                    const settings = {
                        modelName: modelName,
                        learningRate: parseFloat(document.getElementById('learningRate').value),
                        batchSize: parseInt(document.getElementById('batchSize').value)
                    };

                    const response = await fetch('/Dashboard/AIManagement/UpdateModelSettings', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                        },
                        body: JSON.stringify(settings)
                    });

                    const result = await response.json();
                    if (result.success) {
                        this.notify(`${modelName} updated`, 'success');
                    } else {
                        this.notify(result.message || 'Update failed', 'error');
                    }
                } catch (error) {
                    console.error('Error updating settings:', error);
                    this.notify('Update failed', 'error');
                }
            }
        });
    }

    async exportModel(modelName, event) {
        if (event) event.stopPropagation();
        document.getElementById(`options-${modelName}`).classList.add('hidden');

        try {
            this.notify(`Preparing ${modelName} export...`, 'info');

            const response = await fetch('/Dashboard/AIManagement/ExportModel', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ modelName: modelName })
            });

            const result = await response.json();

            if (result.success) {
                const link = document.createElement('a');
                link.href = result.downloadUrl || '#';
                link.download = `${modelName.toLowerCase().replace(' ', '_')}_model.zip`;
                link.click();
                this.notify(`${modelName} exported`, 'success');
            } else {
                this.notify(result.message || 'Export failed', 'error');
            }
        } catch (error) {
            console.error('Error exporting model:', error);
            this.notify('Export failed', 'error');
        }
    }

    async deleteModel(modelName, event) {
        if (event) event.stopPropagation();
        document.getElementById(`options-${modelName}`).classList.add('hidden');

        if (!window.AlertModal) {
            if (!confirm(`Permanently delete ${modelName}?`)) return;
        } else {
            const confirmed = await new Promise(resolve => {
                window.AlertModal.confirm(
                    `Are you sure you want to delete the ${modelName} model? This action is irreversible.`,
                    'Terminate Model',
                    resolve
                );
            });
            if (!confirmed) return;
        }

        try {
            const response = await fetch('/Dashboard/AIManagement/DeleteModel', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                },
                body: JSON.stringify({ modelName: modelName })
            });

            const result = await response.json();
            if (result.success) {
                this.notify(`${modelName} deleted`, 'success');
                setTimeout(() => location.reload(), 1500);
            } else {
                this.notify(result.message || 'Deletion failed', 'error');
            }
        } catch (error) {
            console.error('Error deleting model:', error);
            this.notify('Deletion failed', 'error');
        }
    }

    async viewTrainingDetails(modelName, date) {
        try {
            const response = await fetch(`/Dashboard/AIManagement/GetTrainingDetails?modelName=${encodeURIComponent(modelName)}&date=${encodeURIComponent(date)}`);
            const result = await response.json();

            if (result.success && window.AlertModal) {
                const data = result.data;
                window.AlertModal.show({
                    type: 'info',
                    title: `Session Log - ${modelName}`,
                    message: `
                        <div class="space-y-4 text-left">
                            <div class="grid grid-cols-2 gap-4 text-xs font-bold">
                                <div><span class="opacity-50">Duration:</span> ${data.Duration}</div>
                                <div><span class="opacity-50">Accuracy:</span> ${data.FinalAccuracy}%</div>
                            </div>
                            <div class="p-4 bg-muted/30 border border-border/50 rounded-xl font-mono text-[10px] max-h-40 overflow-y-auto">
                                ${data.TrainingLog.map(log => `<div>${log}</div>`).join('')}
                            </div>
                        </div>
                    `
                });
            }
        } catch (error) {
            console.error('Error loading training details:', error);
        }
    }

    async downloadTrainingReport(modelName, date) {
        try {
            this.notify(`Generating report...`, 'info');
            const response = await fetch(`/Dashboard/AIManagement/DownloadTrainingReport?modelName=${encodeURIComponent(modelName)}&date=${encodeURIComponent(date)}`);
            const result = await response.json();

            if (result.success) {
                const link = document.createElement('a');
                link.href = result.downloadUrl || '#';
                link.download = `${modelName}_report_${date}.pdf`;
                link.click();
                this.notify('Report downloaded', 'success');
            }
        } catch (error) {
            console.error('Error downloading report:', error);
        }
    }

    notify(message, type) {
        if (window.AlertModal) {
            if (type === 'success') window.AlertModal.success(message);
            else if (type === 'error') window.AlertModal.error(message);
            else window.AlertModal.show({ type: 'info', title: 'Notification', message: message });
        } else {
            console.log(`${type.toUpperCase()}: ${message}`);
        }
    }
}

// Global instance
window.aiTraining = new AITrainingManager();
