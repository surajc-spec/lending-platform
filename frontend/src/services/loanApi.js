const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL || "http://localhost:5107/api";

async function handleResponse(response) {
    if (!response.ok) {
        let errorMessage = "Something went wrong.";

        try {
            const errorData = await response.json();

            if (errorData.message) {
                errorMessage = errorData.message;
            } else if (errorData.errors) {
                errorMessage = Object.values(errorData.errors)
                    .flat()
                    .join(" ");
            }
        } catch {
            // Response did not contain JSON.
        }

        throw new Error(errorMessage);
    }

    return response.json();
}

export async function createLoanApplication(data) {
    const response = await fetch(`${API_BASE_URL}/loans`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    });

    return handleResponse(response);
}

export async function getApplications() {
    const response = await fetch(`${API_BASE_URL}/loans`);

    return handleResponse(response);
}

export async function getMetrics() {
    const response = await fetch(`${API_BASE_URL}/loans/metrics`);

    return handleResponse(response);
}