function MetricCard({ label, value, description }) {
    return (
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
            <p className="text-sm font-medium text-slate-500">
                {label}
            </p>

            <p className="mt-3 text-3xl font-bold tracking-tight text-slate-900 tabular-nums">
                {value}
            </p>

            {description && (
                <p className="mt-2 text-sm text-slate-500">
                    {description}
                </p>
            )}
        </div>
    );
}

export default MetricCard;