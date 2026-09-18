function DecisionCard({ result }) {
    const isSuccessful = result.decision === "Successful";

    return (
        <div
            role="region"
            aria-live="polite"
            aria-label="Loan Application Decision Result"
            className={`rounded-2xl border p-6 shadow-sm ${
                isSuccessful
                    ? "border-emerald-200 bg-emerald-50"
                    : "border-red-200 bg-red-50"
            }`}
        >
            <div className="mb-6 flex items-center justify-between">
                <div>
                    <p className="text-sm font-medium text-slate-500">
                        Loan Decision
                    </p>

                    <h2
                        className={`mt-1 text-2xl font-bold ${
                            isSuccessful
                                ? "text-emerald-700"
                                : "text-red-700"
                        }`}
                    >
                        {isSuccessful ? "Successful" : "Declined"}
                    </h2>
                </div>

                <div
                    className={`flex h-12 w-12 items-center justify-center rounded-full text-xl font-bold ${
                        isSuccessful
                            ? "bg-emerald-100 text-emerald-700"
                            : "bg-red-100 text-red-700"
                    }`}
                >
                    {isSuccessful ? "✓" : "×"}
                </div>
            </div>

            <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
                <div className="rounded-xl bg-white/80 p-4">
                    <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                        Loan Amount
                    </p>
                    <p className="mt-1 text-lg font-semibold text-slate-900 tabular-nums">
                        £{Number(result.loanAmount).toLocaleString("en-GB", {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2,
                        })}
                    </p>
                </div>

                <div className="rounded-xl bg-white/80 p-4">
                    <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                        LTV
                    </p>
                    <p className="mt-1 text-lg font-semibold text-slate-900 tabular-nums">
                        {Number(result.ltv).toFixed(2)}%
                    </p>
                </div>

                <div className="rounded-xl bg-white/80 p-4">
                    <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                        Credit Score
                    </p>
                    <p className="mt-1 text-lg font-semibold text-slate-900 tabular-nums">
                        {result.creditScore}
                    </p>
                </div>
            </div>

            <div className="mt-5 border-t border-slate-200/70 pt-5">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
                    Reason
                </p>

                <p className="mt-2 text-sm leading-6 text-slate-700">
                    {result.reason}
                </p>
            </div>
        </div>
    );
}

export default DecisionCard;