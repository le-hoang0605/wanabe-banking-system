import api from './api.js';

// --- Temporary variable for authentication and status changes ---
let pendingAccount = {
    accountNumber: null,
    newStatus: null
};

// --- Modal Controls ---
window.closePasswordModal = function() {
    document.getElementById('passwordModal').classList.add('hidden');
    document.getElementById('confirmPassword').value = '';
};

window.toggleAccountStatus = function(accountNumber, currentStatus) {
    pendingAccount.accountNumber = accountNumber;
    
    pendingAccount.newStatus = currentStatus === 0 ? 1 : 0; 
    
    document.getElementById('passwordModal').classList.remove('hidden');
};

// --- Execute Status Change via API ---
window.executeStatusChange = async function() {
    const password = document.getElementById('confirmPassword').value;
    if (!password) {
        alert("Please enter your password.");
        return;
    }

    try {
        await api.put(`/api/accounts/${pendingAccount.accountNumber}/change-status`, { 
            newStatus: pendingAccount.newStatus,
            password: password 
        });
        
        alert("Status updated successfully!");
        closePasswordModal();
        initProfile(); // Reload the profile data
    } catch (error) {
        console.error('Error updating status:', error);
        alert(error.response?.data?.message || "Failed to update status. Please check your password.");
    }
};

// --- Load Profile & Account Info ---
async function initProfile() {
    try {
        const profileRes = await api.get('/api/Auth/profile');
        document.getElementById('userName').innerText = profileRes.data.fullName;
        document.getElementById('userEmail').innerText = profileRes.data.email;

        const accountsRes = await api.get('/api/accounts/my-accounts');
        renderCards(accountsRes.data);
    } catch (error) {
        console.error('Error loading profile:', error);
    }
}

// --- Render Bank Cards UI ---
function renderCards(accounts) {
    const container = document.getElementById('cardsList');
    if (!container) return;

    container.innerHTML = accounts.map(acc => {
        // FIXED LOGIC: 0 is Active according to your Backend Enum mapping
        const isActive = acc.status === 0; 
        const statusLabel = isActive ? "Active" : "Suspended";
        const statusClass = isActive ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700";

        return `
        <div class="bg-white p-6 rounded-2xl shadow-sm border border-slate-200">
            <p class="text-sm text-slate-400">Account Number</p>
            <p class="text-lg font-mono font-semibold mb-4 text-slate-800">${acc.accountNumber}</p>
            <div class="flex justify-between items-center">
                <div>
                    <p class="text-sm text-slate-400">Balance</p>
                    <p class="text-xl font-bold text-slate-900">${acc.currency || 'VND'} ${acc.balance.toLocaleString()}</p>
                </div>
                <div class="flex flex-col items-end gap-1">
                    <span class="px-3 py-1 text-xs font-bold rounded-full ${statusClass}">${statusLabel}</span>
                    <button onclick="toggleAccountStatus('${acc.accountNumber}', ${acc.status})" 
                            class="text-[10px] text-blue-600 hover:underline font-semibold cursor-pointer">
                        Change Status
                    </button>
                </div>
            </div>
        </div>
        `;
    }).join('');
}

// Initialize on page load
initProfile();