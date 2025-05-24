
interface CheckboxGroupProps {
    groups: string[];
    selectedGroups: string[];
    onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
}
function CheckboxGroup({ groups, selectedGroups, onChange }: CheckboxGroupProps) {
    return (
        <>
            {groups.map((group) => (
                <label key={group}>
                    {group}
                    <input
                        type="checkbox"
                        name="Groups"
                        value={group}
                        checked={selectedGroups.includes(group)}
                        onChange={onChange}
                    />
                </label>
            ))}
        </>
    );
}

export default CheckboxGroup;