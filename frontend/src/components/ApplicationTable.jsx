function ApplicationTable({ applications }) {
    function formatCurrency(value) {
        return Number(value).toLocaleString("en-GB", {
            style: "currency",
            currency: "GBP",
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        });
    }

    function formatDate(value) {
        return new Date(value).toLocaleString("en-GB", {
            dateStyle: "medium",
            timeStyle: "short",
        });
    }

    return (
        <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full text-left">
                    <caption className="sr-only">
                        Historical loan applications and their evaluation decisions
                    </caption>
                    <thead className="border-b border-slate-200 bg-slate-50">
                        <tr>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                ID
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                Loan Amount
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                Asset Value
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                Credit Score
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                LTV
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                Decision
                            </th>
                            <th scope="col" className="px-5 py-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                Date
                            </th>
                        </tr>
                    </thead>

                    <tbody className="divide-y divide-slate-100">
                        {applications.map((application) => {
                            const isSuccessful =
                                application.decision === "Successful";

                            return (
                                <tr
                                    key={application.id}
                                    className="transition hover:bg-slate-50"
                                >
                                    <th scope="row" className="whitespace-nowrap px-5 py-4 text-sm font-medium text-slate-900">
                                        #{application.id}
                                    </th>

                                    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-700 tabular-nums">
                                        {formatCurrency(application.loanAmount)}
                                    </td>

                                    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-700 tabular-nums">
                                        {formatCurrency(application.assetValue)}
                                    </td>

                                    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-700 tabular-nums">
                                        {application.creditScore}
                                    </td>

                                    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-700 tabular-nums">
                                        {Number(application.ltv).toFixed(2)}%
                                    </td>

                                    <td className="whitespace-nowrap px-5 py-4">
                                        <span
                                            className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${
                                                isSuccessful
                                                    ? "bg-emerald-100 text-emerald-700"
                                                    : "bg-red-100 text-red-700"
                                            }`}
                                        >
                                            {application.decision}
                                        </span>
                                    </td>

                                    <td className="whitespace-nowrap px-5 py-4 text-sm text-slate-500 tabular-nums">
                                        {formatDate(application.createdAt)}
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default ApplicationTable;