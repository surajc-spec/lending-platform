import { useEffect, useState } from "react";
import MetricCard from "../components/MetricCard";
import { getMetrics } from "../services/loanApi";

function Dashboard() {
    const [metrics, setMetrics] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadMetrics() {
            try {
                setError("");

                const data = await getMetrics();
                setMetrics(data);
            } catch (err) {
                setError(err.message || "Unable to load dashboard metrics.");
            } finally {
                setLoading(false);
            }
        }

        loadMetrics();
    }, []);

    function formatCurrency(value) {
        return Number(value).toLocaleString("en-GB", {
            style: "currency",
            currency: "GBP",
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        });
    }

    if (loading) {
        return (
            <div>
                <div className="mb-8">
                    <p className="mb-2 text-sm font-semibold uppercase tracking-wider text-slate-500">
                        Lending Platform
                    </p>

                    <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
                        Dashboard
                    </h1>
                </div>

                <div
                    aria-busy="true"
                    aria-live="polite"
                    className="rounded-2xl border border-slate-200 bg-white p-10 text-center shadow-sm"
                >
                    <p className="text-sm font-medium text-slate-600">
                        Loading dashboard...
                    </p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div>
                <div className="mb-8">
                    <p className="mb-2 text-sm font-semibold uppercase tracking-wider text-slate-500">
                        Lending Platform
                    </p>

                    <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
                        Dashboard
                    </h1>
                </div>

                <div
                    role="alert"
                    className="rounded-2xl border border-red-200 bg-red-50 p-6 text-sm text-red-700"
                >
                    {error}
                </div>
            </div>
        );
    }

    return (
        <div>
            <div className="mb-8">
                <p className="mb-2 text-sm font-semibold uppercase tracking-wider text-slate-500">
                    Lending Platform
                </p>

                <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
                    Dashboard
                </h1>

                <p className="mt-3 text-slate-600">
                    Overview of lending applications and portfolio activity.
                </p>
            </div>

            <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-4">
                <MetricCard
                    label="Successful Applications"
                    value={metrics.successfulApplications}
                    description="Applications approved"
                />

                <MetricCard
                    label="Declined Applications"
                    value={metrics.declinedApplications}
                    description="Applications declined"
                />

                <MetricCard
                    label="Loans Written"
                    value={formatCurrency(metrics.totalLoanValueWritten)}
                    description="Total successful loan value"
                />

                <MetricCard
                    label="Mean Average LTV"
                    value={`${Number(metrics.meanAverageLtv).toFixed(2)}%`}
                    description="Across all applications"
                />
            </div>
        </div>
    );
}

export default Dashboard;