import { useState } from "react";
import LoanForm from "../components/LoanForm";
import DecisionCard from "../components/DecisionCard";
import { createLoanApplication } from "../services/loanApi";

function ApplyLoan() {
    const [result, setResult] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    async function handleSubmit(data) {
        setLoading(true);
        setResult(null);
        setError("");

        try {
            const response = await createLoanApplication(data);
            setResult(response);
        } catch (err) {
            setError(err.message || "Unable to evaluate the application.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="mx-auto max-w-5xl">
            <div className="mb-8">
                <p className="mb-2 text-sm font-semibold uppercase tracking-wider text-slate-500">
                    Lending Platform
                </p>

                <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
                    Apply for a Loan
                </h1>

                <p className="mt-3 max-w-2xl text-slate-600">
                    Enter the loan details below to evaluate the application
                    against the lending criteria.
                </p>
            </div>

            <div className="grid gap-6 lg:grid-cols-5">
                <div className="lg:col-span-3">
                    <LoanForm
                        onSubmit={handleSubmit}
                        loading={loading}
                    />

                    {error && (
                        <div
                            role="alert"
                            className="mt-4 rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700"
                        >
                            {error}
                        </div>
                    )}
                </div>

                <div className="lg:col-span-2">
                    {result ? (
                        <DecisionCard result={result} />
                    ) : (
                        <div className="flex h-full min-h-64 items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-white p-6 text-center">
                            <div>
                                <p className="font-semibold text-slate-700">
                                    Decision will appear here
                                </p>
                                <p className="mt-2 text-sm leading-6 text-slate-500">
                                    Submit an application to see the lending
                                    decision, LTV and reason.
                                </p>
                            </div>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default ApplyLoan;