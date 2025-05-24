import React, {useEffect, useState} from 'react';
import axios from 'axios';
import CheckboxGroup from './CheckboxGroup';
import './SubscriptionForm.css';

interface FormData {
    Name?: string;
    Email?: string;
    Groups: string[];
}

function SubscripitonForm() {

    const [formData, setFormData] = useState<FormData>({
        Name: '',
        Email: '',
        Groups: [],
    });
    const [groups, setGroups] = useState<string[]>([]);
    const [formMessage, setFormMessage] = useState('');
    const [messageType, setMessageType] = useState<'' | 'success' | 'error'>('');

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
        if (!validateFormData())
            return;
        
        try {
            const response = await axios.post('/subscribe', formData);
            if (response.status === 200) {
                setFormMessage(response.data);
                setMessageType('success');
                setFormData({
                    Name: '',
                    Email: '',
                    Groups: [],
                });
            }
        } catch (error: any) {
            setFormMessage(
                error.response && error.response.data
                    ? error.response.data
                    : `Wystąpił błąd: ${error.message}`
            );
            setMessageType('error');
        }
    };
    
    const validateFormData = () => {
        if (formData.Name && formData.Email)
            return true;
        
        setFormMessage("Nazwa użytkownika oraz email nie mogą być puste");
        setMessageType('error');
        return false;
    }

    return (
        <div className="Form-wrapper">
            <header className="App-header">
                <h1>Witaj w EmailSubscription!</h1>
                <p>Podaj swoje dane i zaznacz do jakich grup chcesz dołączyć.</p>
            </header>
            <form id="SubscriptionForm">
                <input
                    type="text"
                    name="Name"
                    placeholder="Podaj nazwę użytkownika"
                    onChange={handleInputChange}
                    value={formData.Name || ''}
                />
                <input
                    type="email"
                    name="Email"
                    placeholder="Podaj adres email"
                    onChange={handleInputChange}
                    value={formData.Email || ''}
                />
                <CheckboxGroup
                    groups={groups}
                    selectedGroups={formData.Groups}
                    onChange={handleInputChange}
                />
                <button onClick={handleSubmit} className="Submit-button">Subskrybuj</button>
                <p className={`formMsg ${messageType}`}>{formMessage}</p>
            </form>
        </div>
    );
}

export default SubscripitonForm;