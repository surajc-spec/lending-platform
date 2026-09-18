import { useEffect, useState } from "react";
import ApplicationTable from "../components/ApplicationTable";
import { getApplications } from "../services/loanApi";

function Applications() {
    const [applications, setApplications] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadApplications() {
            try {
                setError("");

                const data = await getApplications();
                setApplications(data);
            } catch (err) {
                setError(
                    err.message || "Unable to load applications."
                );
            } finally {
                setLoading(false);
            }
        }

        loadApplications();
    }, []);

    return (
        <div>
            <div className="mb-8">
                <p className="mb-2 text-sm font-semibold uppercase tracking-wider text-slate-500">
                    Lending Platform
                </p>

                <h1 className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
                    Applications
                </h1>

                <p className="mt-3 text-slate-600">
                    Review previously submitted loan applications and their
                    decisions.
                </p>
            </div>

            {loading && (
                <div
                    aria-busy="true"
                    aria-live="polite"
                    className="rounded-2xl border border-slate-200 bg-white p-10 text-center shadow-sm"
                >
                    <p className="text-sm font-medium text-slate-600">
                        Loading applications...
                    </p>
                </div>
            )}

            {!loading && error && (
                <div
                    role="alert"
                    className="rounded-2xl border border-red-200 bg-red-50 p-6 text-sm text-red-700"
                >
                    {error}
                </div>
            )}

            {!loading && !error && applications.length === 0 && (
                <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center">
                    <h2 className="font-semibold text-slate-800">
                        No applications yet
                    </h2>

                    <p className="mt-2 text-sm text-slate-500">
                        Submitted loan applications will appear here.
                    </p>
                </div>
            )}

            {!loading && !error && applications.length > 0 && (
                <ApplicationTable applications={applications} />
            )}
        </div>
    );
}

export default Applications;