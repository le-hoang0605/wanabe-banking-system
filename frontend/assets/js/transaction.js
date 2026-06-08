// transaction.js
import api from './api.js';

function getHistoryStyles(tx, currentAccountId) {
    const isDeposit = tx.debtorAccountId === '00000000-0000-0000-0000-000000000000' || !tx.debtorAccountId;
    const isDebit = tx.debtorAccountId === currentAccountId && !isDeposit;
    const isWithdraw = tx.creditorAccountId === '00000000-0000-0000-0000-000000000000' || !tx.creditorAccountId;
    
    const displayAmount = (isDebit || isWithdraw ? "-" : "+") + tx.amount.toLocaleString();
    
    let colorClass = (isDebit || isWithdraw) ? 'text-rose-600' : 'text-emerald-600';
    if (tx.status === 'Failed') {
        colorClass = 'text-slate-400 line-through';
    }

    const statusBadge = tx.status === 'Failed' 
        ? `<span class="bg-red-50 text-red-600 px-2 py-0.5 rounded-full text-[10px] font-bold border border-red-100 ml-1.5">Failed</span>`
        : `<span class="bg-emerald-50 text-emerald-700 px-2 py-0.5 rounded-full text-[10px] font-bold border border-emerald-100 ml-1.5">Success</span>`;

   
    let typeText = 'Internal Transfer';
    let typeBadgeClass = 'bg-blue-50 text-blue-700 border-blue-100';
    
    if (isDeposit) {
        typeText = 'Cash Deposit';
        typeBadgeClass = 'bg-emerald-50 text-emerald-700 border-emerald-100';
    } else if (isWithdraw) {
        typeText = 'Cash Withdrawal';
        typeBadgeClass = 'bg-amber-50 text-amber-700 border-amber-100';
    } else if (isDebit) {
        typeText = 'Money Sent';
        typeBadgeClass = 'bg-rose-50 text-rose-700 border-rose-100';
    } else {
        typeText = 'Money Received';
        typeBadgeClass = 'bg-indigo-50 text-indigo-700 border-indigo-100';
    }

    return { displayAmount, colorClass, statusBadge, typeText, typeBadgeClass };
}

let currentPage = 1;
const urlParams = new URLSearchParams(window.location.search);
let accountId = urlParams.get('accountId');

async function loadHistory(page) {
    try {
      
        let url = `/api/transactions/history/${accountId}?pageNumber=${page}&pageSize=10`;
        
        
        const startDate = document.getElementById('startDate').value;
        const endDate = document.getElementById('endDate').value;
        
        if (startDate) url += `&startDate=${startDate}`;
        if (endDate) url += `&endDate=${endDate}`;

        const res = await api.get(url);
        const data = Array.isArray(res.data) ? res.data : (res.data.items || []);
        const body = document.getElementById('ledgerBody'); 
        
        document.getElementById('prevBtn').disabled = (page <= 1);

        if (data.length === 0) {
            body.innerHTML = '<tr><td colspan="4" class="p-8 text-center text-slate-400 text-xs font-medium">No ledger records found for this period.</td></tr>';
            return;
        }

        body.innerHTML = data.map(tx => {
            const { displayAmount, colorClass, statusBadge, typeText, typeBadgeClass } = getHistoryStyles(tx, accountId);
            
            return `
                <tr class="hover:bg-slate-50/80 transition-colors">
                    <td class="p-4 text-xs font-semibold text-slate-600 font-mono">
                        ${new Date(tx.createdAt || tx.date).toLocaleString()}
                    </td>
                    <td class="p-4">
                        <span class="px-2.5 py-1 rounded-lg text-[10px] font-black tracking-wide uppercase border ${typeBadgeClass}">
                            ${typeText}
                        </span>
                    </td>
                    <td class="p-4 text-xs font-medium text-slate-500 max-w-xs truncate">
                        ${tx.description || 'Wanabe Bank Transaction'}
                    </td>
                    <td class="p-4 text-right">
                        <div class="flex items-center justify-end">
                            <span class="font-black text-sm ${colorClass}">${displayAmount} VND</span>
                            ${statusBadge}
                        </div>
                    </td>
                </tr>
            `;
        }).join('');

        document.getElementById('pageInfo').innerText = `Page ${page}`;
    } catch (e) {
        console.error(e);
        alert("Failed to load transaction history.");
    }
}

// --- Khởi tạo dữ liệu trang ban đầu ---
async function initPage() {
    if (!accountId) {
        try {
            const res = await api.get('/api/accounts/my-accounts');
            if (res.data && res.data.length > 0) {
                accountId = res.data[0].accountId;
                console.log("Automatically loaded default account:", accountId);
            } else {
                alert("No bank accounts found to view history!");
                return;
            }
        } catch (err) {
            console.error(err);
            alert("Failed to verify account information.");
            return;
        }
    }
    
    loadHistory(currentPage);
}


document.getElementById('btnFilter').addEventListener('click', () => {
    currentPage = 1; 
    loadHistory(currentPage);
});

document.getElementById('btnClearFilter').addEventListener('click', () => {
    document.getElementById('startDate').value = '';
    document.getElementById('endDate').value = '';
    currentPage = 1;
    loadHistory(currentPage);
});


document.getElementById('prevBtn').addEventListener('click', () => { 
    if (currentPage > 1) { 
        currentPage--; 
        loadHistory(currentPage); 
    } 
});

document.getElementById('nextBtn').addEventListener('click', () => { 
    currentPage++; 
    loadHistory(currentPage); 
});

// Run initialization
initPage();