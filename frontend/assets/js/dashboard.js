// dashboard.js
import api from './api.js';

let loadedTransactions = []; 
let selectedAccountId = null; // KHÓA CHÍ MẠNG: Biến lưu trữ ID của thẻ đang được người dùng chọn

// --- History Display Logic ---
window.getHistoryStyles = function(tx, currentAccountId) {
    const isDeposit = tx.debtorAccountId === '00000000-0000-0000-0000-000000000000' || !tx.debtorAccountId;
    const isDebit = tx.debtorAccountId === currentAccountId && !isDeposit;
    
    const displayAmount = (isDebit ? "-" : "+") + tx.amount.toLocaleString();
    
    let colorClass = isDebit ? 'text-rose-600' : 'text-emerald-600';
    if (tx.status === 'Failed') {
        colorClass = 'text-slate-400 line-through';
    }

    const statusBadge = tx.status === 'Failed' 
        ? `<span class="bg-red-50 text-red-600 px-2 py-0.5 rounded-full text-[10px] font-bold border border-red-100 ml-1.5">Failed</span>`
        : `<span class="bg-emerald-50 text-emerald-700 px-2 py-0.5 rounded-full text-[10px] font-bold border border-emerald-100 ml-1.5">Success</span>`;

    return { displayAmount, colorClass, statusBadge, isDebit, isDeposit };
};

// --- Modal Logic ---
window.closeModal = function () {
    document.getElementById('transactionModal').classList.add('hidden');
};

window.openDepositModal = function() {
    if(!selectedAccountId) { alert("Please select an account first!"); return; }
    document.getElementById('depositModal').classList.remove('hidden');
};
window.closeDepositModal = function() {
    document.getElementById('depositModal').classList.add('hidden');
    document.getElementById('depositAmount').value = '';
};

window.openWithdrawModal = function() {
    if(!selectedAccountId) { alert("Please select an account first!"); return; }
    document.getElementById('withdrawModal').classList.remove('hidden');
};
window.closeWithdrawModal = function() {
    document.getElementById('withdrawModal').classList.add('hidden');
    document.getElementById('withdrawAmount').value = '';
};

window.showTransactionDetail = function (transactionId) {
    try {
        const data = loadedTransactions.find(t => (t.paymentId || t.id || t.transactionId) === transactionId);
        if (!data) {
            alert("Transaction details not found in current view.");
            return;
        }

        const { displayAmount, colorClass, statusBadge, isDebit, isDeposit } = window.getHistoryStyles(data, selectedAccountId);
        
        let typeText = 'Internal Transfer';
        if (isDeposit) typeText = 'Cash Deposit';
        else if (data.creditorAccountId === '00000000-0000-0000-0000-000000000000' || !data.creditorAccountId) typeText = 'Cash Withdrawal';
        else if (isDebit) typeText = 'Money Sent';
        else typeText = 'Money Received';

        const content = document.getElementById('modalContent');
        content.innerHTML = `
            <div class="space-y-3.5 text-xs">
                <div class="bg-slate-50 p-3 rounded-xl border border-slate-100 font-mono text-[11px] text-slate-500 flex justify-between items-center">
                    <span>Transaction ID:</span>
                    <span class="font-bold text-slate-700 select-all">${data.paymentId || data.id}</span>
                </div>
                <div class="flex justify-between items-center py-1.5 border-b border-slate-100">
                    <span class="text-slate-400 font-medium">Type</span>
                    <span class="font-bold text-slate-800 text-sm">${typeText}</span>
                </div>
                <div class="flex justify-between items-center py-1.5 border-b border-slate-100">
                    <span class="text-slate-400 font-medium">Amount</span>
                    <span class="font-black ${colorClass} text-sm">${displayAmount} VND</span>
                </div>
                <div class="flex justify-between items-center py-1.5 border-b border-slate-100">
                    <span class="text-slate-400 font-medium">Timestamp</span>
                    <span class="font-semibold text-slate-600">${new Date(data.createdAt || data.date).toLocaleString()}</span>
                </div>
                <div class="flex flex-col py-1 gap-1.5">
                    <span class="text-slate-400 font-medium">Description</span>
                    <span class="bg-slate-50 p-2.5 rounded-xl text-slate-700 font-medium border border-slate-100 leading-relaxed text-[11px]">${data.description || 'Wanabe Bank Transaction'}</span>
                </div>
                <div class="flex justify-between items-center pt-1">
                    <span class="text-slate-400 font-medium">Ledger Status</span>
                    ${statusBadge}
                </div>
            </div>
        `;
        document.getElementById('transactionModal').classList.remove('hidden');
    } catch (err) {
        console.error("Error showing transaction detail:", err);
        alert("Could not load transaction details.");
    }
};

// --- Authentication & Data Loading ---
function checkAuth() {
    const token = sessionStorage.getItem('jwt_token') || localStorage.getItem('jwt_token');
    if (!token) { window.location.href = 'login.html'; return false; }
    return true;
}

async function loadUserInfo() {
    try {
        const res = await api.get('/api/Auth/profile');
        const nameElement = document.getElementById('navUserFullName');
        if (nameElement) nameElement.innerText = res.data.fullName || "User";
    } catch (e) { console.error("Error loading user:", e); }
}

// CẢI TIẾN MỚI: Tự động vẽ giao diện thẻ và xử lý Active Class khi click
async function loadAccounts() {
    try {
        const res = await api.get('/api/accounts/my-accounts');
        const cardsContainer = document.getElementById('cardsContainer');
        if (!cardsContainer) return;

        // Nếu lần đầu tải trang chưa có thẻ nào được chọn, lấy thẻ đầu tiên làm mặc định
        if (!selectedAccountId && res.data.length > 0) {
            selectedAccountId = res.data[0].accountId;
        }

        // Vẽ danh sách thẻ HTML động
        cardsContainer.innerHTML = res.data.map(acc => {
            const isSelected = acc.accountId === selectedAccountId;
            
            // Nếu thẻ được chọn: viền xanh dương rực rỡ, nền sáng nhẹ. Nếu không chọn: viền xám nhẹ thanh lịch
            const activeClass = isSelected 
                ? 'border-blue-600 bg-blue-50/40 ring-2 ring-blue-500/10 shadow-sm' 
                : 'border-slate-200 bg-slate-50 hover:bg-slate-100/80 hover:border-slate-300';

            return `
                <div onclick="changeSelectedAccount('${acc.accountId}')" 
                     class="group relative cursor-pointer p-4 rounded-xl border-2 transition-all duration-200 ${activeClass}">
                    <div class="flex justify-between items-center">
                        <span class="text-[11px] font-mono tracking-wider font-bold text-slate-500">${acc.accountNumber}</span>
                        ${isSelected ? '<span class="w-2 h-2 bg-blue-600 rounded-full animate-pulse"></span>' : ''}
                    </div>
                    <p class="text-base font-black text-slate-800 mt-2.5">${acc.balance.toLocaleString()} VND</p>
                </div>
            `;
        }).join('');

        // Cập nhật tổng số dư trên giao diện lớn
        const total = res.data.reduce((sum, acc) => sum + acc.balance, 0);
        document.getElementById('balanceDisplay').innerText = `${total.toLocaleString()} VND`;
    } catch (e) { console.error("Error loading accounts:", e); }
}

// HÀM SỰ KIỆN: Khi người dùng click chọn thẻ khác
window.changeSelectedAccount = async function(accountId) {
    selectedAccountId = accountId;
    await loadAccounts();     // Vẽ lại giao diện thẻ để cập nhật viền xanh Active
    await loadTransactions(); // Tải lại lịch sử giao dịch của riêng thẻ vừa chọn
};

async function loadTransactions() {
    const transactionList = document.getElementById('transactionList');
    if (!selectedAccountId) return;

    try {
        const res = await api.get(`/api/transactions/history/${selectedAccountId}?pageNumber=1&pageSize=10`);
        const transactions = Array.isArray(res.data) ? res.data : (res.data.items || []);

        loadedTransactions = transactions; 

        if (!transactions || transactions.length === 0) {
            transactionList.innerHTML = '<p class="text-slate-400 text-xs text-center py-6">No ledger entries found for this account.</p>';
            return;
        }

        transactionList.innerHTML = transactions.map(tx => {
            const { displayAmount, colorClass, statusBadge, isDebit, isDeposit } = window.getHistoryStyles(tx, selectedAccountId);
            const idToUse = tx.paymentId || tx.id || tx.transactionId; 

            let briefType = isDebit ? 'Money Sent' : 'Money Received';
            let iconBg = '';
            let iconSvg = '';

            if (tx.status === 'Failed') {
                iconBg = 'bg-slate-100 text-slate-400';
                iconSvg = `<svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"></path></svg>`;
            } else if (isDeposit) {
                briefType = 'Cash Deposit';
                iconBg = 'bg-emerald-50 text-emerald-600';
                iconSvg = `<svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 4.5l-15 15m0 0h11.25m-11.25 0V8.25"></path></svg>`;
            } else if (tx.creditorAccountId === '00000000-0000-0000-0000-000000000000' || !tx.creditorAccountId) {
                briefType = 'Cash Withdrawal';
                iconBg = 'bg-amber-50 text-amber-600';
                iconSvg = `<svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M4.5 19.5l15-15m0 0H8.25m11.25 0v11.25"></path></svg>`;
            } else if (isDebit) {
                iconBg = 'bg-rose-50 text-rose-600';
                iconSvg = `<svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M4.5 19.5l15-15m0 0H8.25m11.25 0v11.25"></path></svg>`;
            } else {
                iconBg = 'bg-emerald-50 text-emerald-600';
                iconSvg = `<svg class="w-4 h-4" fill="none" stroke="currentColor" stroke-width="2.5" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" d="M19.5 4.5l-15 15m0 0h11.25m-11.25 0V8.25"></path></svg>`;
            }

            return `
                <div onclick="showTransactionDetail('${idToUse}')" 
                     class="group cursor-pointer hover:bg-slate-50 flex justify-between items-center border-b border-slate-100 py-3 px-2 rounded-xl transition-all last:border-b-0">
                    <div class="flex items-center gap-3">
                        <div class="w-9 h-9 rounded-xl ${iconBg} flex items-center justify-center shrink-0 transition-transform group-hover:scale-105 shadow-inner">
                            ${iconSvg}
                        </div>
                        <div>
                            <p class="text-xs font-bold text-slate-800">${briefType}</p>
                            <p class="text-[10px] text-slate-400 mt-0.5">${new Date(tx.createdAt || tx.date).toLocaleString()}</p>
                        </div>
                    </div>
                    <div class="text-right">
                        <p class="text-xs font-black ${colorClass}">${displayAmount} VND</p>
                        ${statusBadge}
                    </div>
                </div>
            `;
        }).join('') + `
            <div class="pt-2">
                <a href="transaction.html?accountId=${selectedAccountId}" 
                   class="block w-full text-center text-xs text-blue-600 font-bold py-2 bg-slate-50 hover:bg-blue-50 rounded-xl transition border border-slate-100 hover:border-blue-100">
                    View All Ledger Entries →
                </a>
            </div>
        `;
    } catch (e) {
        transactionList.innerHTML = '<p class="text-red-500 text-xs p-4 text-center">Unable to load historical ledger.</p>';
    }
}

// --- Main Init ---
document.addEventListener('DOMContentLoaded', async () => {
    if (!checkAuth()) return;
    await loadUserInfo();
    await loadAccounts();      // Hàm này chạy trước để thiết lập selectedAccountId mặc định
    await loadTransactions();  // Sau đó tải lịch sử giao dịch tương ứng
});

// --- Transfer ---
const transferForm = document.getElementById('transferForm');
if (transferForm) {
    transferForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        if(!selectedAccountId) { alert("Please select a source account card!"); return; }
        
        const targetAccountNumber = document.getElementById('targetAccountNumber').value;
        const amount = parseFloat(document.getElementById('transferAmount').value);
        try {
            const accRes = await api.get(`/api/accounts/${targetAccountNumber}`);
            await api.post('/api/transactions/transfer', {
                DebtorAccountId: selectedAccountId, // DÙNG BIẾN TOÀN CỤC ĐANG CHỌN
                CreditorAccountId: accRes.data.accountId,
                Amount: amount
            }, { headers: { 'X-Idempotency-Key': crypto.randomUUID() } });
            alert("Transfer successful!");
            location.reload();
        } catch (e) { alert(e.response?.data?.message || "Transfer failed!"); }
    });
}

// --- Deposit ---
const depositForm = document.getElementById('depositForm');
if (depositForm) {
    depositForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const amount = parseFloat(document.getElementById('depositAmount').value);

        try {
            await api.post('/api/transactions/deposit', {
                CreditorAccountId: selectedAccountId, // DÙNG BIẾN TOÀN CỤC ĐANG CHỌN
                Amount: amount
            }, { headers: { 'X-Idempotency-Key': crypto.randomUUID() } });

            alert("Deposit successful!");
            closeDepositModal();
            location.reload();
        } catch (err) {
            alert(err.response?.data?.message || "Deposit failed!");
        }
    });
}

// --- Withdraw ---
const withdrawForm = document.getElementById('withdrawForm');
if (withdrawForm) {
    withdrawForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const amount = parseFloat(document.getElementById('withdrawAmount').value);

        try {
            await api.post('/api/transactions/withdraw', {
                DebtorAccountId: selectedAccountId, // DÙNG BIẾN TOÀN CỤC ĐANG CHỌN
                Amount: amount
            }, { headers: { 'X-Idempotency-Key': crypto.randomUUID() } });

            alert("Withdrawal successful!");
            closeWithdrawModal();
            location.reload();
        } catch (err) {
            alert(err.response?.data?.message || "Withdrawal failed!");
        }
    });
}

// --- Logout ---
const btnLogout = document.getElementById('btnLogout');
if (btnLogout) {
    btnLogout.addEventListener('click', () => {
        localStorage.removeItem('jwt_token');
        sessionStorage.removeItem('jwt_token');
        window.location.href = 'login.html';
    });
}