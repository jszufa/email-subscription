import React, {useEffect, useState} from 'react';
import axios from 'axios';

interface FormData {
    Name?: string;
    Email?: string;
    Groups: string[];
}
function App() {

    const [formData, setFormData] = useState<FormData>({
        Groups: [],
    });
    const [groups, setGroups] = useState<string[]>([]);
    const [formMessage, setFormMessage] = useState('');

    useEffect(() => {
        getGroups();
    }, [])

    const getGroups = () => {
        axios.get(`/groups`)
            .then((response) => {
                setGroups(response.data);
            })
            .catch((err) => {
                console.error(err)
            });
    }
    const handleInputChange = (
        e: React.ChangeEvent<HTMLInputElement>
    ) => {
        const {name, value, type, checked} = e.target;

        setFormData((prev) => {
            if (type === 'checkbox') {
                const groups = prev.Groups || [];
                return {
                    ...prev,
                    Groups: checked
                        ? [...groups, value]
                        : groups.filter((group) => group !== value),
                };
            }
            return {
                ...prev,
                [name]: value,
            };
        });
    };

    const handleSubmit = async (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();

        try {
            const response = await axios.post('/subscribe', formData);
            if (response.status === 200) {
                setFormMessage(response.data);
            }
        } catch (error: any) {
            setFormMessage(
                error.response && error.response.data
                    ? error.response.data
                    : `Wystąpił błąd: ${error.message}`
            );
        }
    };

    return (
        <div className="App">
            <header className="App-header">
                <h1>Welcome to EmailSubscriptionApp</h1>
            </header>
            <div className="App-body">
                <form>
                    <label>
                        Enter name
                        <input
                            type="text"
                            name="Name"
                            placeholder="ex. John"
                            onChange={handleInputChange}
                        />
                    </label>
                    <label>
                        Enter email
                        <input
                            type="email"
                            name="Email"
                            placeholder="ex. example@ex.com"
                            onChange={handleInputChange}
                        />
                    </label>
                    {groups.map((group) => (
                        <label key={group}>
                            {group}
                            <input
                                type="checkbox"
                                name="Groups"
                                value={group}
                                checked={formData.Groups.includes(group)}
                                onChange={handleInputChange}
                            />
                        </label>
                    ))}
                    <button onClick={handleSubmit}>Wyślij</button>
                    <p className="formMsg">{formMessage}</p>
                </form>
            </div>
        </div>
    );
}

export default App;